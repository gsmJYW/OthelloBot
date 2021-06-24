﻿using Discord.WebSocket;
using System;
using System.Linq;

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

		public int turn = Piece.Red;
		public int[,] board = new int[8, 8];
		public SocketTextChannel channel;
		public SocketUser red, blue;

		public Game(SocketTextChannel channel, SocketUser red, SocketUser blue)
		{
			this.channel = channel;
			this.red = red;
			this.blue = blue;

			InitBoard();
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