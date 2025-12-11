using JiebaNet.Segmenter;
using Kp.LuceneSearchEngine.Interfaces;
using Kp.LuceneSearchEngine.JiebaAnalyzer;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Directory = Lucene.Net.Store.Directory;

namespace Kp.LuceneSearchEngine.Extensions
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="option"></param>
        public static IServiceCollection AddSearchEngine(this IServiceCollection services, LuceneIndexerOptions option)
        {
            services.AddSingleton(option);

            services.AddMemoryCache();

            services.TryAddSingleton<Directory>(s => FSDirectory.Open(option.Path));

            services.TryAddSingleton<Analyzer>(s => new JieBaAnalyzer(TokenizerMode.Search));

            if (option.OnlySearch == true)
            {
                services.TryAddScoped<ILuceneIndexSearcher, LuceneIndexSearcher>();
            }
            else
            {
                services.TryAddSingleton<IndexWriter>(s =>
                {
                    var analyzer = s.GetRequiredService<Analyzer>();
                    var config = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, analyzer)
                    {
                        MaxBufferedDocs = 100, // 最大文档数量
                        OpenMode = OpenMode.CREATE_OR_APPEND, // 默认就是追加模式
                        RAMBufferSizeMB = 256.0, // 减小内存占用，适合Web场景
                        UseCompoundFile = true, // 使用复合文件格式，减少文件数量
                        MergePolicy = new TieredMergePolicy() // 使用分层合并策略
                        {
                            MaxMergeAtOnce = 5,
                            SegmentsPerTier = 10
                        }
                    };
                    var dir = s.GetRequiredService<Directory>();

                    // 检查是否有残留的锁文件
                    var lockFile = Path.Combine(option.Path, IndexWriter.WRITE_LOCK_NAME);
                    if (File.Exists(lockFile))
                    {
                        // 尝试判断是否真的有进程持有锁
                        try
                        {
                            // 尝试打开文件，如果能打开说明锁是残留的
                            using (var fs = new FileStream(lockFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                            {
                                // 如果能打开，说明没有进程持有锁，可以安全删除
                                fs.Close();
                                File.Delete(lockFile);
                                s.GetRequiredService<ILogger<Directory>>()?.LogWarning($"已删除残留的锁文件: {lockFile}");
                            }
                        }
                        catch (IOException)
                        {
                            // 文件被其他进程锁定，说明有另一个实例在运行
                            throw new InvalidOperationException($"索引目录被另一个进程锁定。请确保没有其他实例在运行，或手动删除锁文件: {lockFile}");
                        }
                        catch
                        {
                            throw;
                        }
                    }

                    return new IndexWriter(dir, config);
                });

                services.TryAddScoped<ILuceneIndexer, LuceneIndexer>();
                services.TryAddScoped<ILuceneIndexSearcher, LuceneIndexSearcher>();
                services.TryAddScoped<ISearchEngine, SearchEngine>();
            }

            return services;
        }
    }
}
