import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { ClientAction } from '../classes/ClientAction';
import { GameState, Player } from '../classes/GameState';

const ExampleNames = [
  "Gerhard",
  "Volker",
  "Kai-Udo",
  "Wakandalfsprudo",
  "Peter Zwegert",
  "Gernhardt Reinholzen",
  "Marcel Davis"
];

@Injectable({
  providedIn: 'root'
})
export class ConnectionService {
  private socket: WebSocket;

  public connected = false;
  public error = false;

  public gameState: GameState;
  public gameCode = "";
  public playerName = ExampleNames[Math.floor(Math.random() * ExampleNames.length)];
  public nameError = false;
  public lobbyNotFound = false;

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
    if (this.gameState) {
      data.add("messageid", this.gameState.oCurrentMessage.sMessageID);
    }
    if (!environment.production) {
      (<any>window).DEBUGLastSend = data;
    }
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
    if (id) {
      window.history.pushState({ }, "", url + "!" + this.gameState.sGameId);
    } else {
      window.history.pushState({ }, "", url);
    }
  }

  private onOpen(e: Event) {
    console.log("Wss connection opened");
    this.error = false;
    this.connected = true;
  }

  private onMessage(e: MessageEvent) {
    if (e.data === "name taken") {
      this.nameError = true;
      return;
    }
    if (e.data === "game not found") {
      this.lobbyNotFound= true;
      return;
    }
    this.nameError = false;
    this.lobbyNotFound= false;
    let payload: GameState = JSON.parse(e.data);

    // "player movement interpolation"
    // this doesn't belong here but we don't have events anywhere else in the app

    let maxMoved = 0;
    let changedPlayers = [];
    // carry over old positions to do some animation
    for (let i in payload.oPlayers) {
      let playerNew: Player = payload.oPlayers[i];
      playerNew.shtOldBoardPosition = 0;
      try {
        let playerOld: Player = this.gameState.oPlayers[playerNew.sName];
        playerNew.shtOldBoardPosition = playerOld.shtBoardPosition
        let moved = Math.abs(playerNew.shtOldBoardPosition - playerNew.shtBoardPosition);
        if (moved !== 0) {
          changedPlayers.push(i);
          maxMoved = Math.max(moved, maxMoved);
        }
      } catch (error) {
      }
    }

    // now this is not only absolute garbage, it's probably wrong too
    for (let i = 0; i < maxMoved; i++) {
      setTimeout(() => {
        for (let p of changedPlayers) {
          var player: Player = this.gameState.oPlayers[p];
          const fac = ((i + 1) / maxMoved);
          const lerped = player.shtOldBoardPosition + fac *
            (player.shtBoardPosition - player.shtOldBoardPosition);
          player.shtOldBoardPosition = Math.floor(lerped);

        }
      }, 200 * (i + 1)); // the word hack doesn't do this justice
    }


    this.gameState = payload;
    if (this.gameState.sGameId) {
      this.setUrlId(this.gameState.sGameId);
    } else {
      this.setUrlId(null);
    }
    if (!environment.production) {
      (<any>window).DEBUGSate = this.gameState;
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
