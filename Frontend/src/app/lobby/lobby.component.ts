import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-lobby',
  standalone: true,
  imports: [],
  templateUrl: './lobby.component.html',
  styleUrl: './lobby.component.scss'
})
export class LobbyComponent {
  private router = inject(Router);
  
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
