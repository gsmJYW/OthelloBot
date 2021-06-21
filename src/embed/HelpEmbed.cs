using Discord;

namespace OthelloBot.src.embed
{
    internal class HelpEmbed : EmbedBuilder
    {
        public HelpEmbed(string avatarUrl)
        {
            WithColor(new Color(0xFFFFFF));
            WithThumbnailUrl(avatarUrl);
            AddField($"{Program.prefix}도움말", "오델로봇의 사용법을 알려줍니다.");
        }
    }
}
