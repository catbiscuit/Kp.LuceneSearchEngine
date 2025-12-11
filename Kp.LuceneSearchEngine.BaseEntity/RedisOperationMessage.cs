namespace Kp.LuceneSearchEngine.BaseEntity
{
    public class RedisOperationMessage
    {
        /// <summary>
        /// 实体类型的完整名称 (AssemblyQualifiedName)
        /// </summary>
        public string EntityTypeFullName { get; set; }

        /// <summary>
        /// 实体对象序列化后的 JSON 字符串
        /// </summary>
        public string EntityJson { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public RedisOptEnum RedisOptEnum { get; set; }
    }

    public enum RedisOptEnum
    {
        Add,
        Update,
        Delete,
        Init,
    }
}
