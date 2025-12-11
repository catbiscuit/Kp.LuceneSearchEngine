using Kp.LuceneSearchEngine.BaseEntity;

namespace Kp.LuceneSearchEngine.Entity
{
    public class User : LuceneIndexableBaseEntity
    {
        public User()
        {
            CreateTime = DateTime.Now;
        }

        /// <summary>
        /// 用户名
        /// </summary>
        [LuceneIndex]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [LuceneIndex]
        public string Password { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
    }
}
