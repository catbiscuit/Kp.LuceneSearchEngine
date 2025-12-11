using Kp.LuceneSearchEngine.BaseEntity;

namespace Kp.LuceneSearchEngine.Entity
{
    public class Post : LuceneIndexableBaseEntity
    {
        public Post()
        {
            PostDate = DateTime.Now;
        }

        /// <summary>
        /// 标题
        /// </summary>
        [LuceneIndex]
        public string Title { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        [LuceneIndex]
        public string Author { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [LuceneIndex(IsHtml = true)]
        public string Content { get; set; }

        /// <summary>
        /// 发表时间
        /// </summary>
        public DateTime PostDate { get; set; }

        /// <summary>
        /// 作者邮箱
        /// </summary>
        [LuceneIndex]
        public string Email { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        [LuceneIndex]
        public string Label { get; set; }

        /// <summary>
        /// 文章关键词
        /// </summary>
        [LuceneIndex]
        public string Keyword { get; set; }
    }
}
