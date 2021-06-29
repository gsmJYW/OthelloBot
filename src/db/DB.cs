using Discord.WebSocket;
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

            string query = $"SELECT * FROM user WHERE id={user_id}";
            MySqlDataAdapter adpt = new MySqlDataAdapter(query, conn);

            DataSet ds = new DataSet();
            adpt.Fill(ds, "user");

            try
            {
                return ds.Tables[0].Rows[0];
            }
            catch
            {
                throw;
            }
        }

        public static void UpdateUser(ulong user_id, int win, int draw, int lose, int playtime_second)
        {
            var pre_win = 0;
            var pre_draw = 0;
            var pre_lose = 0;
            var pre_playtime_second = 0;

            try
            {
                var userRow = GetUser(user_id);

                pre_win = Convert.ToInt32(userRow["win"]);
                pre_draw = Convert.ToInt32(userRow["draw"]);
                pre_lose = Convert.ToInt32(userRow["lose"]);
                pre_playtime_second = Convert.ToInt32(userRow["playtime_second"]);
            }
            catch
            {

            }

            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string query = $"REPLACE INTO user (id, win, draw, lose, playtime_second) VALUES({user_id}, {pre_win + win}, {pre_draw + draw}, {pre_lose + lose}, {pre_playtime_second + playtime_second})";
            using var cmd = new MySqlCommand(query, conn);

            cmd.ExecuteNonQuery();
        }
    }
}
