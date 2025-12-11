using Lucene.Net.Index;

namespace Kp.LuceneIndexManager.HostedService
{
    public class LuceneIndexManagerService : IHostedService
    {
        private readonly ILogger<LuceneIndexManagerService> _logger;
        private readonly IndexWriter _indexWriter;

        public LuceneIndexManagerService(ILogger<LuceneIndexManagerService> logger, IndexWriter indexWriter)
        {
            _logger = logger;
            _indexWriter = indexWriter;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("LuceneIndexManagerService StartAsync 索引服务启动。");
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("LuceneIndexManagerService StartAsync 索引服务停止中...");

            try
            {
                _indexWriter?.Commit();
                _indexWriter?.Dispose();

                _logger.LogInformation("LuceneIndexManagerService StartAsync 索引资源已释放。");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "LuceneIndexManagerService StartAsync 索引资源时发生严重错误。");
            }
        }
    }
}
