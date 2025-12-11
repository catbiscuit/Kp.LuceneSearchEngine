using JiebaNet.Segmenter;
using Kp.LuceneSearchEngine.BaseEntity;
using Kp.LuceneSearchEngine.Interfaces;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using Directory = Lucene.Net.Store.Directory;

namespace Kp.LuceneSearchEngine
{
    /// <summary>
    /// 搜索引擎
    /// </summary>
    public class SearchEngine : ISearchEngine
    {
        /// <summary>
        /// 索引器
        /// </summary>
        public ILuceneIndexer LuceneIndexer { get; }

        /// <summary>
        /// 索引搜索器
        /// </summary>
        public ILuceneIndexSearcher LuceneIndexSearcher { get; }

        /// <summary>
        /// 索引条数
        /// </summary>
        public int IndexCount => LuceneIndexer.Count();

        /// <summary>
        /// 搜索引擎
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="analyzer"></param>
        /// <param name="memoryCache"></param>
        public SearchEngine(Directory directory, Analyzer analyzer, IndexWriter writer, IMemoryCache memoryCache)
        {
            LuceneIndexer = new LuceneIndexer(directory, analyzer, writer);
            LuceneIndexSearcher = new LuceneIndexSearcher(directory, analyzer, memoryCache);
        }

        /// <summary>
        /// 创建数据集索引
        /// </summary>
        public void CreateIndex(IEnumerable<ILuceneIndexable> entities, bool recreate = true)
        {
            LuceneIndexer.CreateIndex(entities, recreate);
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        public void DeleteIndex()
        {
            LuceneIndexer?.DeleteAll();
        }

        /// <summary>
        /// 执行搜索并将结果限制为特定类型，在返回之前，搜索结果将转换为相关类型
        /// </summary>
        /// <param name ="options">搜索选项</param>
        /// <returns></returns>
        public IScoredSearchResultCollection<ILuceneIndexable> ScoredSearch(SearchOptions options)
        {
            return ScoredSearch<ILuceneIndexable>(options);
        }

        /// <summary>
        /// 执行搜索并将结果限制为特定类型，在返回之前，搜索结果将转换为相关类型，但不返回任何评分信息
        /// </summary>
        /// <typeparam name ="T">要搜索的实体类型 - 注意：必须实现ILuceneIndexable </typeparam>
        /// <param name ="options">搜索选项</param>
        /// <returns></returns>
        public IScoredSearchResultCollection<T> ScoredSearch<T>(SearchOptions options)
        {
            // 确保类型匹配
            if (typeof(T) != typeof(ILuceneIndexable))
            {
                options.Type = typeof(T);
            }

            var indexResults = LuceneIndexSearcher.ScoredSearch(options);
            IScoredSearchResultCollection<T> results = new ScoredSearchResultCollection<T>();
            results.TotalHits = indexResults.TotalHits;
            var sw = Stopwatch.StartNew();
            foreach (var indexResult in indexResults.Results)
            {
                IScoredSearchResult<T> result = new ScoredSearchResult<T>();
                result.Score = indexResult.Score;
                result.Entity = (T)LuceneIndexSearcher.GetConcreteFromDocument(indexResult.Document);
                results.Results.Add(result);
            }

            sw.Stop();
            results.Elapsed = indexResults.Elapsed + sw.ElapsedMilliseconds;
            return results;
        }

        /// <summary>
        /// 执行搜索并将结果限制为特定类型，在返回之前，搜索结果将转换为相关类型
        /// </summary>
        /// <param name ="options">搜索选项</param>
        /// <returns></returns>
        public ISearchResultCollection<ILuceneIndexable> Search(SearchOptions options)
        {
            return Search<ILuceneIndexable>(options);
        }

        /// <summary>
        /// 执行搜索并将结果限制为特定类型，在返回之前，搜索结果将转换为相关类型，但不返回任何评分信息
        /// </summary>
        /// <typeparam name ="T">要搜索的实体类型 - 注意：必须实现ILuceneIndexable </typeparam>
        /// <param name ="options">搜索选项</param>
        /// <returns></returns>
        public ISearchResultCollection<T> Search<T>(SearchOptions options)
        {
            options.Type = typeof(T);
            var indexResults = LuceneIndexSearcher.ScoredSearch(options);
            ISearchResultCollection<T> resultSet = new SearchResultCollection<T>
            {
                TotalHits = indexResults.TotalHits
            };

            var sw = Stopwatch.StartNew();
            foreach (var indexResult in indexResults.Results)
            {
                var entity = (T)LuceneIndexSearcher.GetConcreteFromDocument(indexResult.Document);
                resultSet.Results.Add(entity);
            }

            sw.Stop();
            resultSet.Elapsed = indexResults.Elapsed + sw.ElapsedMilliseconds;
            return resultSet;
        }

        /// <summary>
        /// 搜索一条匹配度最高的记录
        /// </summary>
        /// <param name ="options">搜索选项</param>
        /// <returns></returns>
        public ILuceneIndexable SearchOne(SearchOptions options)
        {
            return LuceneIndexSearcher.GetConcreteFromDocument(LuceneIndexSearcher.ScoredSearchSingle(options));
        }

        /// <summary>
        /// 搜索一条匹配度最高的记录
        /// </summary>
        /// <param name ="options">搜索选项</param>
        /// <returns></returns>
        public T SearchOne<T>(SearchOptions options) where T : class
        {
            return LuceneIndexSearcher.GetConcreteFromDocument(LuceneIndexSearcher.ScoredSearchSingle(options)) as T;
        }

        /// <summary>
        /// 导入自定义词库
        /// </summary>
        /// <param name="words"></param>
        public void ImportCustomerKeywords(IEnumerable<string> words)
        {
            var segmenter = new JiebaSegmenter();
            foreach (var word in words)
            {
                segmenter.AddWord(word);
            }
        }
    }
}
