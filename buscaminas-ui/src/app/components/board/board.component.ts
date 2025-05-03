import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common'; // Necesario para *ngIf, *ngFor
import { GameService } from '../../services/game.service';
import { Game, Cell } from '../../models/game.model'; // Importa los modelos

@Component({
  selector: 'app-board',
  standalone: true, // Asegúrate que sea standalone
  imports: [CommonModule], // Importa CommonModule aquí
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.scss']
})
export class BoardComponent implements OnInit {
  game: Game | null = null; // Inicializa como null

  constructor(private gameService: GameService) {}

  ngOnInit(): void {
    this.newGame(); // Llama a newGame al inicializar
  }

  newGame(): void {
    // Llama al servicio con valores por defecto o configurables
    this.gameService.startNewGame(8, 8, 10).subscribe((data) => {
      // Mapea la respuesta del backend (Tiles) al frontend (board: Cell[][])
      this.mapBackendResponseToGame(data);
    });
  }

  revealCell(x: number, y: number): void {
    if (!this.game || this.game.isGameOver || this.game.board[x][y].isRevealed || this.game.board[x][y].isFlagged) {
      return; // No hacer nada si el juego terminó, la celda está revelada o tiene bandera
    }
    // Asegúrate de pasar el ID del juego actual
    this.gameService.revealCell(this.game.id, x, y).subscribe((data) => {
       this.mapBackendResponseToGame(data); // Actualiza el tablero con la respuesta
    });
  }

  flagCell(x: number, y: number, event: MouseEvent): void {
    event.preventDefault(); // Previene el menú contextual del navegador
    if (!this.game || this.game.isGameOver || this.game.board[x][y].isRevealed) {
      return; // No hacer nada si el juego terminó o la celda está revelada
    }
     // Asegúrate de pasar el ID del juego actual
    this.gameService.flagCell(this.game.id, x, y).subscribe((data) => {
       this.mapBackendResponseToGame(data); // Actualiza el tablero con la respuesta
    });
  }

  // Función auxiliar para mapear la respuesta del backend al modelo del frontend
  private mapBackendResponseToGame(backendBoard: any): void {
     if (!backendBoard || !backendBoard.tiles) {
         console.error("Respuesta inválida del backend:", backendBoard);
         this.game = null; // O manejar el error apropiadamente
         return;
     }

     const board: Cell[][] = [];
     for (let i = 0; i < backendBoard.height; i++) {
         board[i] = [];
         for (let j = 0; j < backendBoard.width; j++) {
             // Encuentra la 'tile' correspondiente del backend
             const tile = backendBoard.tiles.find((t: any) => t.row === i && t.col === j);
             // Crea la celda del frontend
             board[i][j] = {
                 x: i,
                 y: j,
                 isRevealed: tile?.isRevealed ?? false,
                 isMine: tile?.isMine ?? false,
                 isFlagged: tile?.isFlagged ?? false,
                 adjacentMines: tile?.adjacentMines ?? 0,
             };
         }
     }

     // Actualiza el estado del juego local
     this.game = {
         id: backendBoard.id,
         board: board,
         // Asume que el backend envía estos estados
         isGameOver: backendBoard.isGameOver ?? backendBoard.tiles.some((t: any) => t.isMine && t.isRevealed),
         isWon: backendBoard.isWon ?? false // El backend debería calcular esto
     };
  }

  // Función para obtener la clase CSS de la celda
  getCellClass(cell: Cell): string {
    if (cell.isRevealed) {
      return cell.isMine ? 'mine' : 'revealed adjacent-' + cell.adjacentMines;
    }
    if (cell.isFlagged) {
      return 'flagged';
    }
    return 'hidden'; // Clase por defecto para celdas no reveladas y sin bandera
  }
}