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

            try
            {
                var userRow = DB.GetUser(user.Id);

                var win = Convert.ToInt32(userRow["win"]);
                var draw = Convert.ToInt32(userRow["draw"]);
                var lose = Convert.ToInt32(userRow["lose"]);
                var playtimeSecond = Convert.ToInt32(userRow["playtime_second"]);

                var winRate = (win * 100) / (win + draw + lose);

                var winRank = $"{userRow["win_rank"]}";
                var playtimeSecondRank = $"{userRow["playtime_second_rank"]}";

                AddField("승", $"{win} (#{winRank})", true);
                AddField("무", draw, true);
                AddField("패", lose, true);

                AddField("승률", $"{winRate}%", true);
                AddField("플레이 시간", $"{playtimeSecond / 60}분 (#{playtimeSecondRank})", true);
            }
            catch
            {
                WithDescription("플레이 기록이 없습니다.");
                return;
            }
        }
    }
}
