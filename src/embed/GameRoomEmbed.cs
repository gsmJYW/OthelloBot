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
            WithDescription("참여를 신청하려면 :raised_hand:를 누르세요.\n1분이 지나거나 주최자가 :negative_squared_cross_mark:를 누르면 취소됩니다.");
            WithFooter($"ID {user.Id}");
        }
    }
}
