import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Game } from '../models/game.model'; // Importa la interfaz Game

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private apiUrl = 'http://localhost:5083/api/game'; // Asegúrate que este puerto coincida con tu backend

  constructor(private http: HttpClient) {}

  // Modificado para aceptar y enviar parámetros
  startNewGame(width: number = 8, height: number = 8, mines: number = 10): Observable<Game> {
    // El backend espera un objeto con Width, Height, Mines
    return this.http.post<Game>(`${this.apiUrl}/new`, { Width: width, Height: height, Mines: mines });
  }

  // Modificado para incluir gameId en la URL
  revealCell(gameId: string, x: number, y: number): Observable<Game> {
    // El backend espera Row y Col en el cuerpo
    return this.http.post<Game>(`${this.apiUrl}/${gameId}/reveal`, { Row: x, Col: y });
  }

  // Modificado para incluir gameId en la URL y usar 'toggle-flag'
  flagCell(gameId: string, x: number, y: number): Observable<Game> {
    // El backend espera Row y Col en el cuerpo
    return this.http.post<Game>(`${this.apiUrl}/${gameId}/toggle-flag`, { Row: x, Col: y });
  }
}