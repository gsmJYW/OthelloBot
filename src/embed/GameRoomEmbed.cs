using Discord;
using Discord.WebSocket;

namespace OthelloBot.src.embed
{
    internal class GameRoomEmbed : EmbedBuilder
    {
        public GameRoomEmbed(SocketUser user)
        {
            WithColor(new Color(0xFFFFFF));
            WithTitle($"{user.Username}님의 게임");
            WithDescription("참여를 신청하려면 :raised_hand: 이모지를 누르세요.\n30초가 지나거나 주최자가 :negative_squared_cross_mark: 이모지를 누르면 취소됩니다.");
            WithFooter($"ID {user.Id}");

            AddField("승", 0, true);
            AddField("무", 0, true);
            AddField("패", 0, true);
            AddField("승률", "0%", true);
            AddField("플레이 시간", "0분", true);
        }
    }
}
