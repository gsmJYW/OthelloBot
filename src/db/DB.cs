namespace OthelloBot.src.db
{
    class DB
    {
        private static string connStr;

        public static void SetConnStr(string server, string port, string database, string uid, string pwd)
        {
            connStr = $"Server={server};Port={port};Database={database};Uid={uid};Pwd={pwd};CharSet=utf8;";
        }
    }
}
