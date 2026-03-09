import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

export interface CreateMatchResponse {
    matchId: string;
    matchCode: string;
}

@Injectable({
    providedIn: 'root'
})

export class MatchService {
    private apiUrl = 'https://localhost:7001/api/match';

    constructor(private http: HttpClient) {}

    CreateMatch(): Observable<CreateMatchResponse> {
        return this.http.post<CreateMatchResponse>(this.apiUrl,{});
    }
}
