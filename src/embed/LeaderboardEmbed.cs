using Discord;
using Discord.WebSocket;
using OthelloBot.src.db;
using System;
using System.Data;
using System.Linq;

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

            var winOrderRows = DB.Leaderboard("win");
            var playtimeOrderRows = DB.Leaderboard("playtime_second");

            for (int i = 0; i < winOrderRows.Count; i++)
            {
                var winOrderRow = winOrderRows[i];
                var playtimeOrderRow = playtimeOrderRows[i];

                var win = Convert.ToInt32(winOrderRow["win"]);
                var draw = Convert.ToInt32(winOrderRow["draw"]);
                var lose = Convert.ToInt32(winOrderRow["lose"]);
                var winRate = (win * 100) / (win + draw + lose);

                var playtimeSecond = Convert.ToInt32(playtimeOrderRows[i]["playtime_second"]);

                AddField($"{winOrderRow["name"]}", $"{win}승 ({winRate}%)", true);
                AddField("\u200B", "\u200B", true);
                AddField($"{playtimeOrderRow["name"]}", $"{playtimeSecond / 60}분", true);
            }
        }
    }
}
