using Kp.LuceneIndexManager.HostedService;
using Kp.LuceneSearchEngine.Extensions;
using Kp.LuceneSearchEngine.Interfaces;
using Kp.LuceneSearchEngine.Util;
using NLog.Web;
using SqlSugar;

namespace Kp.LuceneIndexManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSearchEngine(new LuceneSearchEngine.LuceneIndexerOptions()
            {
                Path = UtilConfig.GetLuceneFullPath(),
            });

            // Register the Swagger services
            builder.Services.AddSwaggerDocument(configure =>
            {
                string ProjectName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                configure.PostProcess = document =>
                {
                    document.Info.Version = "v1.0";
                    document.Info.Title = $"{ProjectName}";
                    document.Info.Description = $"{ProjectName}";
                };
                //可以设置从注释文件加载，但是加载的内容可被OpenApiTagAttribute特性覆盖
                configure.UseControllerSummaryAsTagDescription = true;
            });

            builder.Services.AddControllers();

            //注册上下文：AOP里面可以获取IOC对象，如果有现成框架比如Furion可以不写这一行
            builder.Services.AddHttpContextAccessor();
            //注册SqlSugar
            builder.Services.AddSingleton<ISqlSugarClient>(s =>
            {
                SqlSugarScope sqlSugar = new SqlSugarScope(new ConnectionConfig()
                {
                    DbType = DbType.Sqlite,
                    ConnectionString = UtilConfig.GetSqliteFullPath(),
                    IsAutoCloseConnection = true,
                },
               db =>
               {
                   db.Aop.OnLogExecuting = (sql, pars) =>
                   {

                   };
               });
                return sqlSugar;
            });

            string redisConn = "localhost:6379,connectTimeout=5000,defaultDatabase=7";
            RedisHelper.Initialization(new CSRedis.CSRedisClient(redisConn));

            builder.Services.AddHostedService<LuceneIndexManagerService>();
            builder.Services.AddHostedService<RedisBackgroundWorker>();

            builder.Host.UseNLog();

            var app = builder.Build();

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi();

            using (var scope = app.Services.CreateScope())
            {
                var searchEngine = scope.ServiceProvider.GetRequiredService<ISearchEngine>();
                searchEngine.LuceneIndexer.DeleteAll();
            }

            app.MapControllers();

            app.Run();
        }
    }
}
