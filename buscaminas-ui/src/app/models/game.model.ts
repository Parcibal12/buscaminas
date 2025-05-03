// src/app/models/game.model.ts

export interface Cell {
    x: number;
    y: number;
    isRevealed: boolean;
    isMine: boolean;
    isFlagged: boolean;
    adjacentMines: number;
  }
  
  export interface Game {
    id: string;
    board: Cell[][];
    isGameOver: boolean;
    isWon: boolean;
  }
  