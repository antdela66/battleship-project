import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { MatchService, CreateMatchResponse } from '../services/match.services';
import { CommonModule, NgIf } from '@angular/common';

@Component({
  selector: 'app-lobby',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './lobby.component.html',
  styleUrl: './lobby.component.scss'
})
export class LobbyComponent {
  createdMatch: CreateMatchResponse | null = null;
  errormessage = ''; 

  constructor(private matchService: MatchService) {}

  private router = inject(Router);
  
  onCreateGame(): void{

    this.router.navigate(['/join-game']);

    this.matchService.CreateMatch().subscribe({
      next: (response) => {
        this.createdMatch = response;
        console.log('Match Created:', response);
      },
      error: (error) => {
        this.errormessage = 'Failed to create match';
        console.error(error);
      }
    });
  }

  playComputer() {
    console.log("Starting game vs AI...");
    // Logic to start a game against the computer goes here
  }
  
  playPerson() {
    this.router.navigate(['/join-game']);
    console.log("Starting game vs Player...");
  
    // Logic to start a game against another player goes here 


  }
  
}
