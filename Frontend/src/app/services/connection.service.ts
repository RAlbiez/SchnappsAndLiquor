import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { ClientAction } from '../classes/ClientAction';
import { GameState } from '../classes/GameState';

@Injectable({
  providedIn: 'root'
})
export class ConnectionService {
  private socket: WebSocket;

  public connected = false;
  public error = false;

  public gameState: GameState;

  constructor() {
    let location = window.location.href;
    if (environment.production) {
      const index = location.search("!");
      if (index != -1) {
        location = location.slice(0, index);
      }
    } else {
      location = "http://localhost:8080";
    }
    location = location.replace("https://", "wss://");
    location = location.replace("http://", "ws://");
    console.log("Connecting to " + location);
    this.socket = new WebSocket(location);
    this.socket.onopen = (e) => this.onOpen(e);
    this.socket.onmessage = (e) => this.onMessage(e);
    this.socket.onclose = (e) => this.onClose(e);
    this.socket.onerror = (e) => this.onError(e);
  }

  public sendAction(data: ClientAction) {
    this.socket.send(JSON.stringify(data));
  }

  public getIdFromUrl(url = window.location.href) {
    const index = url.search("!");
    if (index == -1) { return null; }
    return url.slice(index + 1);
  }

  public setUrlId(id) {
    let url = window.location.href;
    const index = url.search("!");
    if (index != -1) {
      url = url.slice(0, index);
    }
    window.history.pushState(
      { }, "", url + "!" + this.gameState.sGameId
    );
  }

  private onOpen(e: Event) {
    console.log("Wss connection opened");
    this.error = false;
    this.connected = true;
  }

  private onMessage(e: MessageEvent) {
    let payload = JSON.parse(e.data);
    this.gameState = payload;
    if (this.gameState.sGameId) {
      this.setUrlId(this.gameState.sGameId);
    }
    if (!environment.production) {
      (<any>window).DEBUGSatte = this.gameState;
    }
  }

  private onClose(e: CloseEvent) {
    console.log("Connection closed");
    this.connected = false;
  }

  private onError(e: Event) {
    console.error("Connection failed");
    console.error(e);
    this.connected = false;
    this.error = true;
  }
}
