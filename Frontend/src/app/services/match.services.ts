import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

export interface CreateMatchResponse {
    matchId: string;
    matchCode: string;
}

export interface JoinMatchRequest {
    matchCode: string;
    playerName: string;
}

export interface JoinMatchResponse {
    matchId: string;
    playerId: string;
    playerName: string;
    playerNumber: number;
}
@Injectable({
    providedIn: 'root'
})

export class MatchService {
    private apiUrl = 'https://localhost:7204/api/match';

    constructor(private http: HttpClient) {}

    CreateMatch(): Observable<CreateMatchResponse> {
        return this.http.post<CreateMatchResponse>(this.apiUrl,{});
    }
    CreatePlayer(matchCode: string, playerName: string): Observable<JoinMatchResponse> {
        return this.http.post<JoinMatchResponse>(`${this.apiUrl}/join`, {
            matchCode,
            playerName
        });
    }
    CreatePlayerName(playerId: string, playerName: string): Observable<JoinMatchRequest> {
        return this.http.post<JoinMatchRequest>(`${this.apiUrl}/player/${playerId}/name`, { playerName });
    }
}
