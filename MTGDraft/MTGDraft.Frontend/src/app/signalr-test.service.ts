import { Injectable } from "@angular/core";
import * as signalR from '@microsoft/signalr';

@Injectable({
    providedIn: 'root'
})
export class SignalrTestService {
    private connection!: signalR.HubConnection;

    async startConnection(sessionId: number) {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl('http://localhost:5189/DraftHub')
            .withAutomaticReconnect()
            .build();

        this.connection.on("TimeLeft", (seconds: number) => {
            console.log("Timer:", seconds);
        });

        this.connection.on("PlayerPicked", (playerId: number) => {
            console.log("PlayerPicked:", playerId);
        });

        await this.connection.start();
        console.log("connected to hub");

        await this.connection.invoke("JoinDraft", sessionId)
        console.log("Joined Draft: ", sessionId);
    }
}