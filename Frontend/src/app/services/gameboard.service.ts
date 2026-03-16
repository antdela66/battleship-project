import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

export interface CreateMatchResponse {
    matchId: string;
    matchCode: string;
}

export interface JoinMatchResponse {
    matchId: string;
    playerId: string;
    playerNumber: number;
    playerName: string;
}

export interface JoinMatchRequest {
    matchCode: string;
    playerName: string;
}

export interface PlacementRequest {
    playerId: string;
    shipType: string;
    startX: number;
    startY: number;
    orientation: string;
}

export interface ReadyRequest {
    playerId: string;
}

export interface FireShotRequest {
    playerId: string;
    x: number;
    y: number;
}

export interface FireShotResponse {
    isHit: boolean;
    isSunk: boolean;
    isGameOver: boolean;
    result: string;
    message: string;
    currentTurn: string;
    winner: string;
}

export interface CellState {
    x: number;
    y: number;
    status: string;
}

export interface MoveResult {
    x: number;
    y: number;
    result: string;
    moveTime: string;
}

export interface MatchStateResponse {
    matchCode: string;
    status: string;
    currentTurn: string;
    winner: string;
    playerId: string;
    playerNumber: number;
    playerName: string;
    opponentName: string;
    playerIsReady: boolean;
    opponentIsReady: boolean;
    playerGrid: CellState[];
    opponentGrid: CellState[];
    moves: MoveResult[];
}

@Injectable({
    providedIn: 'root'
})

export class GameBoardService {
    private apiUrl = 'https://localhost:7204/api/match';

    constructor(private http: HttpClient) {}

    getMatchState(matchCode: string, playerId: string): Observable<MatchStateResponse> {
        return this.http.get<MatchStateResponse>(`${this.apiUrl}/${matchCode}/state?playerId=${playerId}`);
    }

    placeShip(matchCode: string, request: PlacementRequest): Observable<any> {
        return this.http.post(`${this.apiUrl}/${matchCode}/placement`, request);
    }

    setReady(matchCode: string, request: ReadyRequest): Observable<any> {
        return this.http.post(`${this.apiUrl}/${matchCode}/ready`, request);
    }

    fireShot(matchCode: string, request: FireShotRequest): Observable<FireShotResponse> {
        return this.http.post<FireShotResponse>(`${this.apiUrl}/${matchCode}/fire`, request);
    }

    getCellState(matchCode: string, playerId: string): Observable<CellState[]> {
        return this.http.get<CellState[]>(`${this.apiUrl}/${matchCode}/board/${playerId}`);
    }

}
