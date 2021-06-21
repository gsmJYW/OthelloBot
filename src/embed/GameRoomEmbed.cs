﻿using Discord;
using Discord.WebSocket;

namespace OthelloBot.src.embed
{
    internal class GameRoomEmbed : EmbedBuilder
    {
        public GameRoomEmbed(SocketUser user)
        {
            WithColor(new Color(0xFFFFFF));
            WithTitle($"{user.Username}님의 게임");
            WithDescription($"참여를 신청하려면 :raised_hand: 이모지를 누르세요.");
            AddField("승", 0, true);
            AddField("패", 0, true);
            AddField("\u200B", "\u200B", true);
            AddField("승률", "0%", true);
            AddField("플레이 시간", "0분", true);
            AddField("\u200B", "\u200B", true);
            WithFooter($"ID {user.Id}");
        }
    }
}
