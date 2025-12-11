using Kp.Entity;
using Kp.LuceneSearchEngine;
using Kp.LuceneSearchEngine.BaseEntity;
using Kp.LuceneSearchEngine.Interfaces;
using Kp.LuceneSearchEngine.Util;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;
using Yitter.IdGenerator;

namespace Kp.Api.Controllers
{
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly ILuceneIndexSearcher _luceneIndexSearcher;
        private readonly ISqlSugarClient _db;

        public HomeController(ILuceneIndexSearcher luceneIndexSearcher, ISqlSugarClient db)
        {
            _luceneIndexSearcher = luceneIndexSearcher;
            _db = db;
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="s">关键词</param>
        /// <param name="page">第几页</param>
        /// <param name="size">页大小</param>
        /// <param name="isHightLight">高亮</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index(string s, int page, int size, bool isHightLight)
        {
            var result = _luceneIndexSearcher.ScoredSearch(new SearchOptions(s, page, size, isHightLight, typeof(LuceneSearchEngine.Entity.Post)));
            return Ok(result);
        }

        /// <summary>
        /// 创建索引
        /// </summary>
        [HttpGet]
        public void CreateIndex()
        {
            var posts = _db.Queryable<Post>().ToList();

            var message = new RedisOperationMessage
            {
                EntityTypeFullName = typeof(LuceneSearchEngine.Entity.Post).AssemblyQualifiedName,
                EntityJson = JsonConvert.SerializeObject(posts),
                RedisOptEnum = RedisOptEnum.Init
            };

            RedisHelper.LPush(UtilConst.RedisKey, message);
        }

        /// <summary>
        /// 添加索引
        /// </summary>
        [HttpPost]
        public void AddIndex(Post p)
        {
            p.Id = YitIdHelper.NextId();
            int rows = _db.Insertable(p).ExecuteCommand();

            var message = new RedisOperationMessage
            {
                EntityTypeFullName = typeof(LuceneSearchEngine.Entity.Post).AssemblyQualifiedName,
                EntityJson = JsonConvert.SerializeObject(new List<Post> { p }),
                RedisOptEnum = RedisOptEnum.Add
            };

            RedisHelper.LPush(UtilConst.RedisKey, message);
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        [HttpDelete]
        public void DeleteIndex(long id)
        {
            Post post = _db.Queryable<Post>().First(x => x.Id == id);
            if (post == null) return;

            _db.Deleteable<Post>(x => x.Id == id).ExecuteCommand();

            var message = new RedisOperationMessage
            {
                EntityTypeFullName = typeof(LuceneSearchEngine.Entity.Post).AssemblyQualifiedName,
                EntityJson = JsonConvert.SerializeObject(new List<Post> { post }),
                RedisOptEnum = RedisOptEnum.Delete
            };

            RedisHelper.LPush(UtilConst.RedisKey, message);
        }

        /// <summary>
        /// 更新索引库
        /// </summary>
        /// <param name="post"></param>
        [HttpPatch]
        public void UpdateIndex(Post p)
        {
            Post post = _db.Queryable<Post>().First(x => x.Id == p.Id);
            if (post == null) return;

            ObjectExtensions.CopyProperties(p, post);

            _db.Updateable(post).ExecuteCommand();

            var message = new RedisOperationMessage
            {
                EntityTypeFullName = typeof(LuceneSearchEngine.Entity.Post).AssemblyQualifiedName,
                EntityJson = JsonConvert.SerializeObject(new List<Post> { post }),
                RedisOptEnum = RedisOptEnum.Update
            };

            RedisHelper.LPush(UtilConst.RedisKey, message);
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Post> GetList()
        {
            var posts = _db.Queryable<Post>().ToList();

            return posts;
        }

        /// <summary>
        /// 语句搜索
        /// </summary>
        /// <param name="s">关键词，示例：Title:积极 AND Content:便利 AND Author:(alltoolsdetail OR chatglm) </param>
        /// <param name="page">第几页</param>
        /// <param name="size">页大小</param>
        /// <param name="isHightLight">高亮</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ScriptSearch(string s, int page, int size, bool isHightLight)
        {
            var result = _luceneIndexSearcher.ScriptSearch<LuceneSearchEngine.Entity.Post>(new SearchOptions(s, page, size, isHightLight, typeof(LuceneSearchEngine.Entity.Post))
            {
                Score = 0.0001f,
            });
            return Ok(result);
        }
    }
}
