using System;
using System.Collections.Generic;
namespace Minesweeper.Api.Models;
public class Board {
    public Guid Id { get; init; } = Guid.NewGuid();
    public int Width { get; set; }
    public int Height { get; set; }
    public List<Tile> Tiles { get; set; } = new List<Tile>();
    public bool IsGameOver { get; set; } // Added for game over state
    public bool IsWon { get; set; }      // Add this property for win state
}