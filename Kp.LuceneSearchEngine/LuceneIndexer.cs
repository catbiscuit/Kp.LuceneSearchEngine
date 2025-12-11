using Kp.LuceneSearchEngine.BaseEntity;
using Kp.LuceneSearchEngine.Interfaces;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Directory = Lucene.Net.Store.Directory;

namespace Kp.LuceneSearchEngine
{
    /// <summary>
    /// 创建索引
    /// </summary>
    public class LuceneIndexer : ILuceneIndexer
    {
        /// <summary>
        /// 索引目录
        /// </summary>
        private readonly Directory _directory;

        /// <summary>
        /// 索引分析器
        /// </summary>
        private readonly Analyzer _analyzer;

        /// <summary>
        /// 索引操作器
        /// </summary>
        private readonly IndexWriter _writer;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="analyzer"></param>
        public LuceneIndexer(Directory directory, Analyzer analyzer, IndexWriter indexWriter)
        {
            _directory = directory;
            _analyzer = analyzer;
            _writer = indexWriter;
        }

        /// <summary>
        /// 添加到索引
        /// </summary>
        /// <param name="entity">实体</param>
        public void Add(ILuceneIndexable entity)
        {
            _writer.AddDocument(entity.ToDocument());

            _writer.Flush(true, true);
            _writer.Commit();
        }

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="entities">实体集</param>
        /// <param name="recreate">是否需要覆盖</param>
        public void CreateIndex(IEnumerable<ILuceneIndexable> entities, bool recreate = true)
        {
            // 删除重建
            if (recreate)
            {
                _writer.DeleteAll();
                _writer.Commit();
            }

            // 遍历实体集，添加到索引库
            foreach (var entity in entities)
            {
                _writer.AddDocument(entity.ToDocument());
            }

            _writer.Flush(true, true);
            _writer.Commit();
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="entity">实体</param>
        public void Delete(ILuceneIndexable entity)
        {
            var type = entity.GetType();
            if (type.Assembly.IsDynamic && type.FullName.Contains("Prox"))
            {
                type = type.BaseType;
            }

            _writer.DeleteDocuments(new Term(LuceneIndexableConst.FieldNameIndexId, type.FullName + entity.Id));

            _writer.Flush(true, true);
            _writer.Commit();
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="entries">实体集</param>
        public void Delete<T>(IList<T> entries) where T : ILuceneIndexable
        {
            foreach (var entity in entries)
            {
                var type = entity.GetType();
                if (type.Assembly.IsDynamic && type.FullName.Contains("Prox"))
                {
                    type = type.BaseType;
                }

                _writer.DeleteDocuments(new Term(LuceneIndexableConst.FieldNameIndexId, type.FullName + entity.Id));
            }

            _writer.Flush(true, true);
            _writer.Commit();
        }

        /// <summary>
        /// 删除所有索引
        /// </summary>
        /// <param name="commit">是否提交</param>
        public void DeleteAll(bool commit = true)
        {
            try
            {
                _writer.DeleteAll();
                if (commit)
                {
                    _writer.Commit();
                }

                _writer.Flush(true, true);
                _writer.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 更新索引
        /// </summary>
        /// <param name="entity">实体</param>
        public void Update(ILuceneIndexable entity)
        {
            var type = entity.GetType();
            if (type.Assembly.IsDynamic && type.FullName.Contains("Prox"))
            {
                type = type.BaseType;
            }

            _writer.DeleteDocuments(new Term(LuceneIndexableConst.FieldNameIndexId, type.FullName + entity.Id));
            _writer.AddDocument(entity.ToDocument());

            _writer.Flush(true, true);
            _writer.Commit();
        }

        /// <summary>
        /// 更新索引-删除索引时仅利用IndexId去删除
        /// </summary>
        /// <param name="changeset">实体</param>
        public void Update<T>(IList<T> entriest) where T : ILuceneIndexable
        {
            foreach (var entity in entriest)
            {
                var type = entity.GetType();
                if (type.Assembly.IsDynamic && type.FullName.Contains("Prox"))
                {
                    type = type.BaseType;
                }

                _writer.DeleteDocuments(new Term(LuceneIndexableConst.FieldNameIndexId, type.FullName + entity.Id));
                _writer.AddDocument(entity.ToDocument());
            }

            _writer.Flush(true, true);
            _writer.Commit();
        }

        /// <summary>
        /// 索引库数量
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            try
            {
                IndexReader reader = DirectoryReader.Open(_directory);
                return reader.NumDocs;
            }
            catch (IndexNotFoundException ex)
            {
                _directory.ClearLock(IndexWriter.WRITE_LOCK_NAME);
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
    }
}
