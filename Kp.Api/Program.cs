using Kp.Entity;
using Kp.LuceneSearchEngine.BaseEntity;
using Kp.LuceneSearchEngine.Extensions;
using Kp.LuceneSearchEngine.Util;
using Newtonsoft.Json;
using NLog.Web;
using SqlSugar;
using Yitter.IdGenerator;

namespace Kp.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Set Yitter.IdGenerator
            var options = new IdGeneratorOptions(workerId: 1);
            YitIdHelper.SetIdGenerator(options);

            builder.Services.AddSearchEngine(new LuceneSearchEngine.LuceneIndexerOptions()
            {
                Path = UtilConfig.GetLuceneFullPath(),
                OnlySearch = true,
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
#if DEBUG
                       Console.WriteLine(sql, pars);
#endif
                   };
               });
                return sqlSugar;
            });

            string redisConn = "localhost:6379,connectTimeout=5000,defaultDatabase=7";
            RedisHelper.Initialization(new CSRedis.CSRedisClient(redisConn));

            builder.Host.UseNLog();

            var app = builder.Build();

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi();

            var db = app.Services.GetService<ISqlSugarClient>();
            db.DbMaintenance.CreateDatabase();
            db.CodeFirst.InitTables(typeof(Post));
            db.CodeFirst.InitTables(typeof(User));

            //先删除数据再重新添加
            db.Deleteable<Post>().ExecuteCommand();
            var posts = JsonConvert.DeserializeObject<List<Post>>(File.ReadAllText(AppContext.BaseDirectory + "Posts.json"));
            db.Insertable(posts).ExecuteCommand();

            //发布redis任务
            var message = new RedisOperationMessage
            {
                EntityTypeFullName = typeof(LuceneSearchEngine.Entity.Post).AssemblyQualifiedName,
                EntityJson = JsonConvert.SerializeObject(posts),
                RedisOptEnum = RedisOptEnum.Init
            };
            RedisHelper.LPush(UtilConst.RedisKey, message);

            app.MapControllers();

            app.Run();
        }
    }
}
