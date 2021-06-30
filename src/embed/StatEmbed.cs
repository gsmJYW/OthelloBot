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

            int win, draw, lose, playtimeSecond;
            double winRate;
            string winRank, playtimeSecondRank;

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
            catch
            {
                WithDescription("플레이 기록이 없습니다.");
                return;
            }

            AddField("승", $"{win} (#{winRank})", true);
            AddField("무", draw, true);
            AddField("패", lose, true);

            AddField("승률", $"{winRate * 100: 0}%", true);
            AddField("플레이 시간", $"{playtimeSecond / 60}분 (#{playtimeSecondRank})", true);
        }
    }
}
