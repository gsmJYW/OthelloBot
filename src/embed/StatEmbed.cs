using Discord;
using Discord.WebSocket;
using OthelloBot.src.db;
using System;

namespace OthelloBot.src.embed
{
    class StatEmbed : EmbedBuilder
    {
        public StatEmbed(SocketUser user)
        {
            var avatarUrl = $"https://cdn.discordapp.com/embed/avatars/{user.DiscriminatorValue % 5}.png";

            if (user.GetAvatarUrl() != null)
            {
                avatarUrl = user.GetAvatarUrl();
            }

            WithAuthor(user.Username, iconUrl: avatarUrl);
            WithColor(new Color(0xFFFFFF));

            var win = 0;
            var draw = 0;
            var lose = 0;
            var playtimeSecond = 0;
            var winRate = 0.0;

            var winRank = "?";
            var playtimeSecondRank = "?";

            try
            {
                var userRow = DB.GetUser(user.Id);

                win = Convert.ToInt32(userRow["win"]);
                draw = Convert.ToInt32(userRow["draw"]);
                lose = Convert.ToInt32(userRow["lose"]);
                playtimeSecond = Convert.ToInt32(userRow["playtime_second"]);

                winRate = (double)win / (win + draw + lose);

                winRank = $"{userRow["win_rank"]}";
                playtimeSecondRank = $"{userRow["playtime_second_rank"]}";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            AddField("승", $"{win} (#{winRank})", true);
            AddField("무", draw, true);
            AddField("패", lose, true);

            AddField("승률", $"{winRate * 100: 0}%", true);
            AddField("플레이 시간", $"{playtimeSecond / 60}분 (#{playtimeSecondRank})", true);
        }
    }
}
