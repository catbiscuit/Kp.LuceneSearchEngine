using Kp.LuceneSearchEngine.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace Kp.LuceneIndexManager.Controllers
{
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly ISearchEngine _searchEngine;
        private readonly ISqlSugarClient _db;

        public HomeController(ISearchEngine searchEngine, ISqlSugarClient db)
        {
            _searchEngine = searchEngine;
            _db = db;
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Info()
        {
            var result = _searchEngine.IndexCount;

            return Ok(new { Data = result, });
        }
    }
}
