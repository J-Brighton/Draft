import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SignalrTestService } from './signalr-test.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('MTGDraft.Frontend');

  constructor(private signalr: SignalrTestService) {}

  startTest() {
    this.signalr.startConnection(1);
  }
}
