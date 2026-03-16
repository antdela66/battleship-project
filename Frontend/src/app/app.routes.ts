import { Routes } from '@angular/router';
import { GameBoardComponent } from './game-board/game-board.component';
import { LobbyComponent } from './lobby/lobby.component';

export const routes: Routes = [
    {path: '', component: LobbyComponent},
    {path: 'game-board/:MatchId/:playerId', component: GameBoardComponent}
];
