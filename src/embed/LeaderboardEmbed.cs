using Discord;
using Discord.WebSocket;
using OthelloBot.src.db;
using System;
using System.Data;

namespace OthelloBot.src.embed
{
    class LeaderboardEmbed : EmbedBuilder
    {
        public LeaderboardEmbed()
        {
            WithColor(new Color(0xFFFFFF));

            AddField("승리 횟수", "\u200B", true);
            AddField("\u200B", "\u200B", true);
            AddField("플레이 시간", "\u200B", true);

            DataTable winOrderTable = DB.Leaderboard("win");
            DataTable playtimeOrderTable = DB.Leaderboard("playtime_second");

            for (int i = 0; i < winOrderTable.Rows.Count; i++)
            {
                var winOrderUser = Program._client.GetUser(Convert.ToUInt64(winOrderTable.Rows[i]["id"]));
                var playtimeOrderUser = Program._client.GetUser(Convert.ToUInt64(playtimeOrderTable.Rows[i]["id"]));

                var win = Convert.ToInt32(winOrderTable.Rows[i]["win"]);
                var draw = Convert.ToInt32(winOrderTable.Rows[i]["draw"]);
                var lose = Convert.ToInt32(winOrderTable.Rows[i]["lose"]);
                var winRate = (win * 100) / (win + draw + lose);

                var playtimeSecond = Convert.ToInt32(playtimeOrderTable.Rows[i]["playtime_second"]);

                AddField($"#{i + 1:00} {winOrderUser.Username}", $"{win}승 ({winRate}%)", true);
                AddField("\u200B", "\u200B", true);
                AddField($"#{i + 1:00} {playtimeOrderUser.Username}", $"{playtimeSecond / 60}분", true);
            }
        }
    }
}
