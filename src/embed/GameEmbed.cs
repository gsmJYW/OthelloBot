using Discord;

namespace OthelloBot.src.embed
{
    internal class GameEmbed : EmbedBuilder
    {
        public GameEmbed(Game game)
        {
            WithColor(new Color(0xFFFFFF));

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
                            if (game.IsAvailable(game.turn, y, x))
                            {
                                boardString += ":white_check_mark:";
                            }
                            else
                            {
                                boardString += ":black_large_square:";
                            }
                            break;
                    }
                }
            }

            WithDescription(boardString);

            string redName = game.red.Username;
            string blueName = game.blue.Username;

            if (game.turn == Game.Piece.Red)
            {
                redName = $"__{redName}__";
            }
            else if (game.turn == Game.Piece.Blue)
            {
                blueName = $"__{blueName}__";
            }

            AddField($"{redName}", $"🔴 {game.CountPiece(Game.Piece.Red):00}\n{game.red_seconds}초", true);
            AddField($"{blueName}", $"🔵 {game.CountPiece(Game.Piece.Blue):00}\n{game.blue_seconds}초", true);

            WithFooter("🙌를 누르면 기권합니다.");
        }
    }
}
