using Discord;

namespace OthelloBot.src.embed
{
    internal class HelpEmbed : EmbedBuilder
    {
        public HelpEmbed(string avatarUrl)
        {
            WithColor(new Color(0xFFFFFF));
            WithThumbnailUrl(avatarUrl);
            AddField($"{Program.prefix}도움말", "오델로 봇의 사용법을 알려줍니다.");
            AddField($"{Program.prefix}게임", "게임을 열고 참여할 사람을 기다립니다.");
            AddField($"{Program.prefix}전적", "자신의 게임 전적을 확인합니다.");
        }
    }
}
