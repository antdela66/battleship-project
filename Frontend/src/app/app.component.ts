import { Component } from '@angular/core';
import { RouterOutlet, RouterModule } from '@angular/router';
import { GameBoardComponent } from './game-board/game-board.component';
import { LobbyComponent } from './lobby/lobby.component';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterModule, GameBoardComponent, LobbyComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})


export class AppComponent {

isGameBoardRoute() {
if (window.location.pathname.startsWith('/join-game/') || window.location.pathname === '/play-computer') {
return true;
}
throw new Error('Unexpected route: ' + window.location.pathname);
}

  title = 'BattleShip_Frontend';
}
