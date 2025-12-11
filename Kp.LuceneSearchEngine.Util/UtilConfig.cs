namespace Kp.LuceneSearchEngine.Util
{
    public class UtilConfig
    {
        public static string GetSqliteFullPath()
        {
            var projectRootPath = Directory.GetCurrentDirectory();

            var sharedDbRelativePath = "..\\sharedData";

            var sharedDataFullPath = Path.GetFullPath(Path.Combine(projectRootPath, sharedDbRelativePath));

            if (Directory.Exists(sharedDataFullPath) == false)
                Directory.CreateDirectory(sharedDataFullPath);

            var dbFileName = "kpDatabase.db";
            var dbFullPath = Path.Combine(sharedDataFullPath, dbFileName);
            var connectionString = $"Data Source={dbFullPath};";

            return connectionString;
        }

        public static string GetLuceneFullPath()
        {
            var projectRootPath = Directory.GetCurrentDirectory();

            var luceneDataRelativePath = "..\\luceneData";

            var luceneDataFullPath = Path.GetFullPath(Path.Combine(projectRootPath, luceneDataRelativePath));

            if (Directory.Exists(luceneDataFullPath) == false)
                Directory.CreateDirectory(luceneDataFullPath);

            return luceneDataFullPath;
        }
    }
}
