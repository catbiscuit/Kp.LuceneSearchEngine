using Kp.LuceneSearchEngine.BaseEntity;
using Kp.LuceneSearchEngine.Interfaces;
using Kp.LuceneSearchEngine.Util;
using Newtonsoft.Json;

namespace Kp.LuceneIndexManager.HostedService
{
    public class RedisBackgroundWorker : IHostedService, IDisposable
    {
        private readonly string _listKey = UtilConst.RedisKey;
        private Task _executingTask;
        private CancellationTokenSource _cts;

        private readonly ILogger<RedisBackgroundWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        public RedisBackgroundWorker(ILogger<RedisBackgroundWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("进入 StartAsync");

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _executingTask = ExecuteAsync(_cts.Token);
            return Task.CompletedTask;
        }

        private async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("进入 ExecuteAsync");
            int errorCount = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (stoppingToken.IsCancellationRequested) return;

                    await Task.Delay(5000, stoppingToken);

                    var message = RedisHelper.LIndex<RedisOperationMessage>(_listKey, 0);
                    if (message != null)
                    {
                        bool result = ProcessItem(message);
                        if (result)
                        {
                            _ = RedisHelper.LPop(_listKey);
                            errorCount = 0;
                        }
                        else
                        {
                            errorCount++;
                        }

                        if (errorCount >= 5)
                        {
                            _logger.LogDebug("业务执行5次均失败");
                            await Task.Delay(1000 * 10, stoppingToken);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogDebug(" OperationCanceledException 结束");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug($" 执行异常 {ex.Message}");
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }

        private bool ProcessItem(RedisOperationMessage message)
        {
            if (message == null) return true;

            var entityType = Type.GetType(message.EntityTypeFullName);
            if (entityType == null)
            {
                _logger.LogError($"Error: Could not find type '{message.EntityTypeFullName}'.");
                return false;
            }

            var listType = typeof(List<>).MakeGenericType(entityType);

            var entityListObject = JsonConvert.DeserializeObject(message.EntityJson, listType);

            if (entityListObject is IEnumerable<ILuceneIndexable> indexableEntities)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var searchEngine = scope.ServiceProvider.GetRequiredService<ISearchEngine>() ?? throw new InvalidOperationException("无法从请求服务中获取ISearchEngine实例");
                    switch (message.RedisOptEnum)
                    {
                        case RedisOptEnum.Add:
                            searchEngine.LuceneIndexer.CreateIndex(indexableEntities, false);
                            break;
                        case RedisOptEnum.Update:
                            searchEngine.LuceneIndexer.Update(indexableEntities.ToList());
                            break;
                        case RedisOptEnum.Delete:
                            searchEngine.LuceneIndexer.Update(indexableEntities.ToList());
                            break;
                        case RedisOptEnum.Init:
                            searchEngine.CreateIndex(indexableEntities, true);
                            break;
                        default:
                            break;
                    }
                }

                return true;
            }
            else
            {
                _logger.LogError($"Error: Deserialized object is not a list of ILuceneIndexable.");
                return false;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("进入 StopAsync");

            _cts?.Cancel();

            if (_executingTask == null) return;

            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }

        public void Dispose()
        {
            _cts?.Dispose();
        }
    }
}
