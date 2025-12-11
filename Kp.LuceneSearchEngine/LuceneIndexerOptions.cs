namespace Kp.LuceneSearchEngine
{
    /// <summary>
    /// 索引器选项
    /// </summary>
    public class LuceneIndexerOptions
    {
        /// <summary>
        /// 索引路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 只需要搜索
        /// </summary>
        public bool? OnlySearch { get; set; }
    }
}
