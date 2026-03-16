import { Component, inject, NgModule, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { LobbyComponent } from '../lobby/lobby.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GameBoardService, MatchStateResponse, CellState } from '../services/gameboard.service';

@Component({
  selector: 'game-board',
  standalone: true,
  imports: [ CommonModule, RouterModule, FormsModule],
  templateUrl: './game-board.component.html',
  styleUrl: './game-board.component.scss'
})

export class GameBoardComponent implements OnInit {
  matchState: MatchStateResponse | null = null;
  matchCode: string = '';
  playerId: string = '';
  actionMessage: string = '';
  errorMessage: string = '';
  shipTypes = ['Carrier', 'Battleship', 'Cruiser', 'Submarine', 'Destroyer'];
  shipOrientations = ['Horizontal', 'Vertical'];

  selectedShip: string = 'Carrier';
  selectedOrientation: string = 'Horizontal';

  board: CellState[] = [];
  placedShips: string[] = [];

  constructor(private gameBoardService: GameBoardService) {}
  
  private router = inject(Router);

  ngOnInit(): void {
    this.matchCode = localStorage.getItem('matchCode') || '';
    this.playerId = localStorage.getItem('playerId') || '';

    if(!this.matchCode || !this.playerId) {
      this.errorMessage = 'Missing match code or player ID. Please join a game from the lobby.';
      console.error('Missing match code or player ID in local storage.');
      return;
    }

    this.gameBoardService.getMatchState(this.matchCode, this.playerId).subscribe({
      next: (response) => {
        console.log('Match State loaded', response);
        this.matchState = response;
        this.loadMatchState();
        this.loadBoard();
      },
      error: (error) => {
        this.errorMessage = 'Failed to fetch match state';
        console.error('Fetch match state error', error);
      }
    })

    
  }

  loadBoard(): void {
    this.gameBoardService.getCellState(this.matchCode, this.playerId).subscribe({
      next: (response) => {
        this.board = response;
        console.log('Board loaded', response);
      },
      error: (error) => {
        this.errorMessage = 'Error fetching board state.';
        console.error('Error fetching board state:', error);
      }
    })
  }

  loadMatchState(): void {
    this.gameBoardService.getMatchState(this.matchCode, this.playerId).subscribe({
      next: (response) => {
        this.matchState = response;
        console.log('Match State:', response);
      },
      error: (error) => {
        this.errorMessage = 'Error fetching match state.';
        console.error('Error fetching match state:', error);
      }
    });
  }

  yourBoard() {
    return this.board;
  }

  opponentBoard() {
    return this.matchState?.opponentGrid ?? [];
   }

  placeShip(x: number, y: number): void {
    if (!this.matchCode || !this.playerId) {
      this.errorMessage = 'Missing match code or player ID. Please join a game from the lobby.';
      console.error('Missing match code or player ID in local storage.');
      return;
    }

    console.log("Placing ship...");
    this.gameBoardService.placeShip(this.matchCode, {
      playerId: this.playerId,
      shipType: this.selectedShip,
      startX: x,
      startY: y, 
      orientation: this.selectedOrientation 
    }).subscribe({
      next: (response) => {
        console.log("Ship placed successfully.");
        this.placedShips.push(this.selectedShip);
        this.loadMatchState();
        this.loadBoard();
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Error placing ship.';
        console.error("Error placing ship:", error);
      }
    });
  }
  
  playerReady():  void{
    if (!this.matchCode || !this.playerId) {
      this.errorMessage = 'Missing match code or player ID. Please join a game from the lobby.';
      console.error('Missing match code or player ID in local storage.');
      return;
    }
    this.gameBoardService.setReady(this.matchCode, {
      playerId: this.playerId
    }).subscribe({
      next: () => {
        console.log("Player is ready.");
        this.loadMatchState();
        this.loadBoard();
      },
      error: (error) => {
        this.errorMessage = 'Error setting player ready.';
        console.error("Error setting player ready:", error);
      }
    });

     }

  getCellStatus(x: number, y: number): string {
    const cell = this.board.find(c =>c.x === x && c.y === y);
    return cell ? cell.status.toLowerCase() : 'empty';
    }
  
   fireShot(row: number, col: number) {
      if (!this.matchCode || !this.playerId) {
        this.errorMessage = 'Missing match code or player ID. Please join a game from the lobby.';
        console.error('Missing match code or player ID in local storage.');
        return;
      }

      if (this.matchState?.currentTurn !== this.matchState?.playerNumber.toString()) {
        this.errorMessage = 'It is not your turn.';
        console.warn('Attempted to fire shot out of turn.');
        return;
      }

      if (this.matchState?.status !== 'InProgress') {
        this.errorMessage = 'The game is not in progress.';
        console.warn('Attempted to fire shot when game is not in progress.');
        return;
      }

      this.gameBoardService.fireShot(this.matchCode, {
        playerId: this.playerId,
        x: row,
        y: col
      }).subscribe({
        next: (response) => {
          console.log("Shot fired successfully. Shot fired at:", {row, col });
          this.actionMessage = response.message;
          this.loadMatchState();
          this.loadBoard();
        },
        error: (error) => {
          this.errorMessage = 'Error firing shot.';
          console.error("Error firing shot:", error);
        }
      });

    }

}