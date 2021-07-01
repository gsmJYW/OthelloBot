using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace OthelloBot.src.db
{
    class DB
    {
        private static string connStr;

        public static void SetConnStr(string server, string port, string database, string uid, string pwd)
        {
            connStr = $"Server={server};Port={port};Database={database};Uid={uid};Pwd={pwd};CharSet=utf8;";
        }

        public static DataRow GetUser(ulong user_id)
        {
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string query = $"SELECT * FROM (SELECT *, RANK() OVER (ORDER BY win DESC) win_rank, RANK() OVER (ORDER BY playtime_second DESC) playtime_second_rank FROM user) as user_with_rank WHERE id = {user_id}";
            MySqlDataAdapter adpt = new MySqlDataAdapter(query, conn);

            DataSet ds = new DataSet();
            adpt.Fill(ds, "user_with_rank");

            try
            {
                return ds.Tables[0].Rows[0];
            }
            catch
            {
                throw;
            }
        }

        public static DataRowCollection GetUsers(string username)
        {
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string query = $"SELECT * FROM (SELECT *, RANK() OVER (ORDER BY win DESC) win_rank, RANK() OVER (ORDER BY playtime_second DESC) playtime_second_rank FROM user) as user_with_rank WHERE name LIKE '%{username}%'";
            MySqlDataAdapter adpt = new MySqlDataAdapter(query, conn);

            DataSet ds = new DataSet();
            adpt.Fill(ds, "user_with_rank");

            try
            {
                return ds.Tables[0].Rows;
            }
            catch
            {
                throw;
            }
        }

        public static DataRowCollection Leaderboard(string order_column)
        {
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string query = $"SELECT * FROM user ORDER BY {order_column} DESC LIMIT 10";
            MySqlDataAdapter adpt = new MySqlDataAdapter(query, conn);

            DataSet ds = new DataSet();
            adpt.Fill(ds, "user");

            try
            {
                return ds.Tables[0].Rows;
            }
            catch
            {
                throw;
            }
        }

        public static void UpdateUser(ulong userId, string username, int win, int draw, int lose, int playtimeSecond)
        {
            var preWin = 0;
            var preDraw = 0;
            var preLose = 0;
            var prePlaytimeSecond = 0;

            try
            {
                var userRow = GetUser(userId);

                preWin = Convert.ToInt32(userRow["win"]);
                preDraw = Convert.ToInt32(userRow["draw"]);
                preLose = Convert.ToInt32(userRow["lose"]);
                prePlaytimeSecond = Convert.ToInt32(userRow["playtime_second"]);
            }
            catch
            {

            }

            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string query = $"REPLACE INTO user (id, name, win, draw, lose, playtime_second) VALUES({userId}, '{username}', {preWin + win}, {preDraw + draw}, {preLose + lose}, {prePlaytimeSecond + playtimeSecond})";
            using var cmd = new MySqlCommand(query, conn);

            cmd.ExecuteNonQuery();
        }
    }
}
