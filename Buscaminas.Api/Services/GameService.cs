using Minesweeper.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Api.Services;
public interface IGameService {
    Board CreateBoard(int width, int height, int mines);
    Board Reveal(Guid gameId, int row, int col);
    Board ToggleFlag(Guid gameId, int row, int col);
}

public class GameService : IGameService {
    private readonly Dictionary<Guid, Board> _games = new();
    private readonly Random _random = new(); // Add Random instance

    // Add the missing CreateBoard method implementation
    public Board CreateBoard(int width, int height, int mines) {
        var board = new Board {
            Width = width,
            Height = height,
            IsGameOver = false, // Initialize game state
            IsWon = false
        };

        // 1. Create all tiles
        for (int r = 0; r < height; r++) {
            for (int c = 0; c < width; c++) {
                board.Tiles.Add(new Tile { Row = r, Col = c });
            }
        }

        // 2. Place mines randomly
        int minesPlaced = 0;
        while (minesPlaced < mines) {
            int r = _random.Next(height);
            int c = _random.Next(width);
            var tile = board.Tiles.Single(t => t.Row == r && t.Col == c);
            if (!tile.IsMine) {
                tile.IsMine = true;
                minesPlaced++;
            }
        }

        // 3. Calculate adjacent mines for each non-mine tile
        foreach (var tile in board.Tiles.Where(t => !t.IsMine)) {
            int adjacentMines = 0;
            for (int rOffset = -1; rOffset <= 1; rOffset++) {
                for (int cOffset = -1; cOffset <= 1; cOffset++) {
                    if (rOffset == 0 && cOffset == 0) continue; // Skip self
                    int neighborRow = tile.Row + rOffset;
                    int neighborCol = tile.Col + cOffset;

                    // Check if neighbor is within bounds and is a mine
                    if (neighborRow >= 0 && neighborRow < height &&
                        neighborCol >= 0 && neighborCol < width &&
                        board.Tiles.Single(t => t.Row == neighborRow && t.Col == neighborCol).IsMine) {
                        adjacentMines++;
                    }
                }
            }
            tile.AdjacentMines = adjacentMines;
        }

        _games[board.Id] = board; // Store the new game
        return board;
    }


    public Board Reveal(Guid gameId, int row, int col) {
        if (!_games.TryGetValue(gameId, out var board)) {
            // Handle case where gameId is not found, perhaps throw an exception or return null/error
            // For now, let's assume it exists or throw
             throw new KeyNotFoundException($"Game with ID {gameId} not found.");
        }

        // Prevent action if game is already over
        if (board.IsGameOver || board.IsWon) return board;

        var target = board.Tiles.SingleOrDefault(t => t.Row == row && t.Col == col);

        // Handle case where tile is not found (shouldn't happen with valid row/col)
        if (target == null) throw new ArgumentOutOfRangeException($"Invalid coordinates ({row}, {col})");

        // Can't reveal flagged or already revealed tiles
        if (target.IsFlagged || target.IsRevealed) return board;

        target.IsRevealed = true;

        if (target.IsMine) {
            // Game Over - Lost
            board.IsGameOver = true;
            // Optionally reveal all mines
            board.Tiles.Where(t => t.IsMine).ToList().ForEach(t => t.IsRevealed = true);
        } else {
             // Check for win condition *after* revealing
             // Win condition: All non-mine tiles are revealed
             bool allNonMinesRevealed = board.Tiles.Where(t => !t.IsMine).All(t => t.IsRevealed);
             if (allNonMinesRevealed) {
                 board.IsWon = true;
                 board.IsGameOver = true; // Game also ends on win
             } else if (target.AdjacentMines == 0) {
                 // Reveal neighbors only if the game is not won yet
                 // Use a queue for non-recursive flood fill to avoid stack overflow on large empty areas
                 var queue = new Queue<Tile>();
                 queue.Enqueue(target);

                 while(queue.Count > 0)
                 {
                     var current = queue.Dequeue();
                     // Iterate over neighbors
                     for (int rOffset = -1; rOffset <= 1; rOffset++)
                     {
                         for (int cOffset = -1; cOffset <= 1; cOffset++)
                         {
                             if (rOffset == 0 && cOffset == 0) continue; // Skip self

                             int neighborRow = current.Row + rOffset;
                             int neighborCol = current.Col + cOffset;

                             // Find the neighbor tile
                             var neighbor = board.Tiles.SingleOrDefault(t => t.Row == neighborRow && t.Col == neighborCol);

                             // If neighbor exists, is not revealed, and not flagged
                             if (neighbor != null && !neighbor.IsRevealed && !neighbor.IsFlagged)
                             {
                                 neighbor.IsRevealed = true;
                                 // If the neighbor is also empty, add it to the queue to process its neighbors
                                 if (neighbor.AdjacentMines == 0)
                                 {
                                     queue.Enqueue(neighbor);
                                 }
                             }
                         }
                     }
                 }
             }
        }
        return board;
    }

    public Board ToggleFlag(Guid gameId, int row, int col) {
         if (!_games.TryGetValue(gameId, out var board)) {
             throw new KeyNotFoundException($"Game with ID {gameId} not found.");
         }
        // Prevent action if game is already over
        if (board.IsGameOver || board.IsWon) return board;

        var tile = board.Tiles.SingleOrDefault(t => t.Row == row && t.Col == col);
        if (tile == null) throw new ArgumentOutOfRangeException($"Invalid coordinates ({row}, {col})");

        if (!tile.IsRevealed) {
            tile.IsFlagged = !tile.IsFlagged;
        }
        return board;
    }

    // Helper to check win condition (optional, logic moved inside Reveal)
    // private bool CheckWinCondition(Board board) {
    //    return board.Tiles.Where(t => !t.IsMine).All(t => t.IsRevealed);
    // }
}