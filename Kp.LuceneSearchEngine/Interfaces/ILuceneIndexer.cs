using Kp.LuceneSearchEngine.BaseEntity;

namespace Kp.LuceneSearchEngine.Interfaces
{
    /// <summary>
    /// 创建索引
    /// </summary>
    public interface ILuceneIndexer
    {
        /// <summary>
        /// 添加到索引
        /// </summary>
        /// <param name="entity">实体</param>
        void Add(ILuceneIndexable entity);

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="entities">实体集</param>
        /// <param name="recreate">是否需要覆盖</param>
        void CreateIndex(IEnumerable<ILuceneIndexable> entities, bool recreate = true);

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="entity">实体</param>
        void Delete(ILuceneIndexable entity);

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="entries">实体集</param>
        void Delete<T>(IList<T> entries) where T : ILuceneIndexable;

        /// <summary>
        /// 删除所有索引
        /// </summary>
        /// <param name="commit">是否提交</param>
        void DeleteAll(bool commit = true);

        /// <summary>
        /// 更新索引
        /// </summary>
        /// <param name="entity">实体</param>
        void Update(ILuceneIndexable entity);

        /// <summary>
        /// 更新索引-删除索引时仅利用IndexId去删除
        /// </summary>
        /// <param name="changeset">实体</param>
        void Update<T>(IList<T> entriest) where T : ILuceneIndexable;

        /// <summary>
        /// 索引库数量
        /// </summary>
        /// <returns></returns>
        int Count();
    }
}
