using Kp.LuceneSearchEngine.BaseEntity;

namespace Kp.LuceneSearchEngine.Interfaces
{
    /// <summary>
    /// 搜索引擎
    /// </summary>
    public interface ISearchEngine
    {
        /// <summary>
        /// 索引器
        /// </summary>
        ILuceneIndexer LuceneIndexer { get; }

        /// <summary>
        /// 索引搜索器
        /// </summary>
        ILuceneIndexSearcher LuceneIndexSearcher { get; }

        /// <summary>
        /// 索引总数
        /// </summary>
        int IndexCount { get; }

        /// <summary>
        /// 创建数据集索引
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="recreate"></param>
        void CreateIndex(IEnumerable<ILuceneIndexable> entities, bool recreate = true);

        /// <summary>
        /// 删除索引
        /// </summary>
        void DeleteIndex();

        /// <summary>
        /// 执行搜索并将结果限制为特定类型，在返回之前，搜索结果将转换为相关类型
        /// </summary>
        /// <param name ="options">搜索选项</param>
        IScoredSearchResultCollection<ILuceneIndexable> ScoredSearch(SearchOptions options);

        /// <summary>
        /// 执行搜索并将结果限制为特定类型，在返回之前，搜索结果将转换为相关类型
        /// </summary>
        /// <typeparam name ="T">要搜索的实体类型 - 注意：必须实现ILuceneIndexable </typeparam>
        /// <param name ="options">搜索选项</param>
        IScoredSearchResultCollection<T> ScoredSearch<T>(SearchOptions options);

        /// <summary>
        /// 执行搜索并将结果限制为特定类型，在返回之前，搜索结果将转换为相关类型，但不返回任何评分信息
        /// </summary>
        /// <param name ="options">搜索选项</param>
        /// <returns></returns>
        ISearchResultCollection<ILuceneIndexable> Search(SearchOptions options);

        /// <summary>
        /// 执行搜索并将结果限制为特定类型，在返回之前，搜索结果将转换为相关类型，但不返回任何评分信息
        /// </summary>
        /// <typeparam name ="T">要搜索的实体类型 - 注意：必须实现ILuceneIndexable </typeparam>
        /// <param name ="options">搜索选项</param>
        /// <returns></returns>
        ISearchResultCollection<T> Search<T>(SearchOptions options);

        /// <summary>
        /// 搜索一条匹配度最高的记录
        /// </summary>
        /// <param name ="options">搜索选项</param>
        ILuceneIndexable SearchOne(SearchOptions options);

        /// <summary>
        /// 搜索一条匹配度最高的记录
        /// </summary>
        /// <param name ="options">搜索选项</param>
        T SearchOne<T>(SearchOptions options) where T : class;

        /// <summary>
        /// 导入自定义词库
        /// </summary>
        /// <param name="words"></param>
        void ImportCustomerKeywords(IEnumerable<string> words);
    }
}
