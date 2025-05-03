// src/app/app.component.ts

import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';                  
import { BoardComponent } from './components/board/board.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,    // Necesario para *ngIf, *ngFor en tu template
    BoardComponent   // Así Angular reconoce <app-board>
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'minesweeper-ui';
}
