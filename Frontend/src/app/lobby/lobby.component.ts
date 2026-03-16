import { Component, inject, NgModule } from '@angular/core';
import { Router } from '@angular/router';
import { MatchService, CreateMatchResponse } from '../services/match.services';
import { CommonModule, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-lobby',
  standalone: true,
  imports: [CommonModule, NgIf, FormsModule],
  templateUrl: './lobby.component.html',
  styleUrl: './lobby.component.scss'
})
export class LobbyComponent {
  createdMatch: CreateMatchResponse | null = null;
  errormessage = ''; 
  matchCode: string = '';
  playerName: string = '';

  constructor(private matchService: MatchService) {}

  private router = inject(Router);

  onCreateGame(): void{
    this.matchService.CreateMatch().subscribe({

      next: (response) => {
        this.createdMatch = response;
        console.log('Match Created:', response);

        this.matchService.CreatePlayer(response.matchCode, this.playerName).subscribe({
          next: (playerResponse) => {
            console.log('Player Created:', playerResponse);

            localStorage.setItem('matchCode', response.matchCode);
            localStorage.setItem('playerId', playerResponse.playerId);

            this.router.navigate([
              '/game-board/',
              response.matchCode,
              playerResponse.playerId
            ]);
          },
          error: (error) => {
            this.errormessage = 'Failed to create player';
            console.error(error);
          }
        });
      },
      error: (error) => {
        this.errormessage = 'Failed to create match';
        console.error(error);
      }
    });

  }

  onJoinGame(matchCode: string): void {
    if (!matchCode) {
      this.errormessage = 'Please enter a match code';
      return;
    }
    this.matchService.CreatePlayer(matchCode, this.playerName).subscribe({
          next: (playerResponse) => {
            console.log('Player Created:', playerResponse);

            localStorage.setItem('matchCode', matchCode);
            localStorage.setItem('playerId', playerResponse.playerId);
            
            this.router.navigate([
              '/game-board/',
              matchCode,
              playerResponse.playerId
            ]);
          },
          error: (error) => {
            this.errormessage = 'Failed to join match. Please check the match code and try again.';
            console.error(error);
      }
    });
  }
}
