import { Routes } from '@angular/router';
import { GameBoardComponent } from './game-board/game-board.component';
import { LobbyComponent } from './lobby/lobby.component';

export const routes: Routes = [
    {path: '', component: LobbyComponent},
    {path: 'join-game/:MatchId', component: GameBoardComponent},
    {path: 'play-computer', component: GameBoardComponent}

];
