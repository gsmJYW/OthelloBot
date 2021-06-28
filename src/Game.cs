using Discord.Rest;
using Discord.WebSocket;
using OthelloBot.src.embed;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace OthelloBot.src
{
	internal class Game
	{
		public static class Piece
		{
			public const int Empty = 0;
			public const int Red = 1;
			public const int Blue = 2;
		}

		public int red_seconds = 300, blue_seconds = 300;
		public int turn = Piece.Red;
		public int[,] board = new int[8, 8];

		public ulong hostId;

		public SocketUser red, blue;
		public RestTextChannel channel;
		public SocketTextChannel roomChannel;
		private RestUserMessage message;

		public Timer timer = new Timer();

		public void SetMessage(RestUserMessage message)
		{
			this.message = message;
		}

		public async Task<RestUserMessage> GetMessage()
		{
			if (message.Embeds.Count == 0)
			{
				await GameEventHandler.RemoveGame(message.Channel.Id);
			}

			return message;
		}

		public Game(SocketUser red, SocketUser blue)
		{
			this.red = red;
			this.blue = blue;

			InitBoard();

			timer.Elapsed += OnTimedEvent;
			timer.Interval = 1000;
			timer.Start();
		}

		private async void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
			if (turn == Piece.Red)
            {
				red_seconds--;

				if (red_seconds % 5 == 0)
				{
					await message.ModifyAsync(msg =>
					{
						var embed = new GameEmbed(this);
						msg.Embed = embed.Build();
					});
				}
			}
			else
			{
				blue_seconds--;

				if (blue_seconds % 5 == 0)
				{
					await message.ModifyAsync(msg =>
					{
						var embed = new GameEmbed(this);
						msg.Embed = embed.Build();
					});
				}
			}

			if (red_seconds == 0 || blue_seconds == 0)
            {
				await GameEventHandler.RemoveGame(hostId);
				
				turn = Piece.Empty;

				var embed = new GameEmbed(this);
				var winner = red_seconds > blue_seconds ? red : blue;
				embed.Title = $"{winner.Username}님이 시간승";
				embed.Footer.Text = "";

				try
				{
					await roomChannel.SendMessageAsync(embed: embed.Build());
				}
				catch (Exception ex)
                {
					Console.WriteLine(ex.Message);
                }
			}
		}

		public void InitBoard()
		{
			Array.Clear(board, Piece.Empty, board.Length);

			board[3, 3] = Piece.Red; board[3, 4] = Piece.Blue;
			board[4, 3] = Piece.Blue; board[4, 4] = Piece.Red;
		}

		public bool IsAvailable(int piece, int row, int col)
		{
			int opponentPiece = Opponent(piece);

			if (board[row, col] != Piece.Empty)
			{
				return false;
			}

			for (int rowDirection = -1; rowDirection <= 1; rowDirection++)
			{
				for (int colDirection = -1; colDirection <= 1; colDirection++)
				{
					if (rowDirection == 0 && colDirection == 0)
					{
						continue;
					}

					try
					{
						if (board[row + rowDirection, col + colDirection] == opponentPiece)
						{
							int tempRow = row;
							int tempCol = col;

							while (true)
							{
								tempRow += rowDirection;
								tempCol += colDirection;

								if (board[tempRow, tempCol] == Piece.Empty)
								{
									break;
								}
								else if (board[tempRow, tempCol] == piece)
								{
									return true;
								}
							}
						}
					}
					catch (IndexOutOfRangeException)
					{
						continue;
					}
				}
			}
			return false;
		}

		public bool HasAvailablePlace(int piece)
		{
			for (int row = 0; row < 8; row++)
			{
				for (int col = 0; col < 8; col++)
				{
					if (IsAvailable(piece, row, col))
					{
						return true;
					}
				}
			}
			return false;
		}

		public void PlacePiece(int piece, int row, int col)
		{
			int opponentPiece = Opponent(piece);

			board[row, col] = piece;

			for (int rowDirection = -1; rowDirection <= 1; rowDirection++)
			{
				for (int colDirection = -1; colDirection <= 1; colDirection++)
				{
					if (rowDirection == 0 && colDirection == 0)
					{
						continue;
					}

					try
					{
						if (board[row + rowDirection, col + colDirection] == opponentPiece)
						{
							int tempRow = row;
							int tempCol = col;

							while (true)
							{
								tempRow += rowDirection;
								tempCol += colDirection;

								if (board[tempRow, tempCol] == Piece.Empty)
								{
									break;
								}
								else if (board[tempRow, tempCol] == piece)
								{
									while (row != tempRow || col != tempCol)
									{
										board[tempRow, tempCol] = piece;

										tempRow -= rowDirection;
										tempCol -= colDirection;
									}

									break;
								}
							}
						}
					}
					catch (IndexOutOfRangeException)
					{
						continue;
					}
				}
			}
		}

		public SocketUser TurnUser()
        {
			if (turn == Piece.Blue)
            {
				return blue;
            }
			else
            {
				return red;
            }
        }

		public int CountPiece(int piece)
		{
			return board.Cast<int>().Count(value => value == piece);
		}

		public static int Opponent(int piece)
		{
			if (piece == Piece.Red)
			{
				return Piece.Blue;
			}
			else
			{
				return Piece.Red;
			}
		}
	}
}