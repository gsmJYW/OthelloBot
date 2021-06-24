using Discord;
using Discord.WebSocket;

namespace OthelloBot.src.embed
{
    internal class GameEmbed : EmbedBuilder
    {
        public GameEmbed(Game game)
        {
            WithColor(new Color(0xFFFFFF));

            string redName = game.red.Username;
            string blueName = game.blue.Username;

            if (game.turn == Game.Piece.Red)
            {
                redName = $"__{redName}__";
            }
            else
            {
                blueName = $"__{blueName}__";
            }

            WithTitle($":red_circle: {game.CountPiece(Game.Piece.Red)} {redName}\n:blue_circle: {game.CountPiece(Game.Piece.Blue)} {blueName}");

            var boardString = ":black_large_square::one::two::three::four::five::six::seven::eight:";

            for (int y = 0; y < 8; y++)
            {
                boardString += $"\n:regional_indicator_{(char)('a' + y)}:";

                for (int x = 0; x < 8; x++)
                {
                    switch (game.board[y, x])
                    {
                        case Game.Piece.Red:
                            boardString += ":red_circle:";
                            break;

                        case Game.Piece.Blue:
                            boardString += ":blue_circle:";
                            break;

                        default:
                            boardString += ":black_large_square:";
                            break;
                    }
                }
            }

            WithDescription(boardString);
        }
    }
}
