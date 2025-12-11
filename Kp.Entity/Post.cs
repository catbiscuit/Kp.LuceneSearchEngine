using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace Kp.Entity
{
    public class Post
    {
        public Post()
        {
            PostDate = DateTime.Now;
        }

        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Required(ErrorMessage = "文章标题不能为空！")]
        public string Title { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        [Required, MaxLength(24, ErrorMessage = "作者名最长支持24个字符！")]
        public string Author { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Required(ErrorMessage = "文章内容不能为空！")]
        public string Content { get; set; }

        /// <summary>
        /// 发表时间
        /// </summary>
        public DateTime PostDate { get; set; }

        /// <summary>
        /// 作者邮箱
        /// </summary>
        [Required(ErrorMessage = "作者邮箱不能为空！")]
        public string Email { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        [StringLength(256, ErrorMessage = "标签最大允许255个字符")]
        public string Label { get; set; }

        /// <summary>
        /// 文章关键词
        /// </summary>
        [StringLength(256, ErrorMessage = "文章关键词最大允许255个字符")]
        public string Keyword { get; set; }
    }
}
