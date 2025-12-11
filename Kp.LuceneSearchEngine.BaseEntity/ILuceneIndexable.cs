using Lucene.Net.Documents;
using System.ComponentModel.DataAnnotations;

namespace Kp.LuceneSearchEngine.BaseEntity
{
    public interface ILuceneIndexable
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [LuceneIndex(Name = "Id", Store = Field.Store.YES), Key]
        long Id { get; set; }

        /// <summary>
        /// 转换成Lucene文档
        /// </summary>
        /// <returns></returns>
        Document ToDocument();
    }
}
