import { Component } from '@angular/core';
import { RouterOutlet, RouterModule } from '@angular/router';
import { LobbyComponent } from '../lobby/lobby.component';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

@Component({
  selector: 'game-board',
  standalone: true,
  imports: [ CommonModule, RouterOutlet, RouterModule],
  templateUrl: './game-board.component.html',
  styleUrl: './game-board.component.scss'
})

export class GameBoardComponent {
  board: number[][] = Array(10).fill(0).map(() => Array(10).fill(0));

  fireShot(row: number, col: number) {
    console.log(`Shot fired at: ${row}, ${col}`);
    // Update logic for hits/misses here
  }

  getCellClass(row: number, col: number) {
    // Return classes based on board state
    return {
      'empty': this.board[row][col] === 0,
      'ship': this.board[row][col] === 1,
      'miss': this.board[row][col] === 2,
      'hit': this.board[row][col] === 3
    };
  }
}
