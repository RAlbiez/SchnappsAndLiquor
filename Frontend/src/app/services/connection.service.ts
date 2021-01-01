import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Actions, ClientAction } from '../classes/ClientAction';

@Injectable({
  providedIn: 'root'
})
export class ConnectionService {
  private socket: WebSocket;
  private subscribers = new Map<Actions, ((data) => {})[]>();

  public connected = false;
  public error = false;

  public gameState = null;

  constructor() {
    let location = "";
    if (environment.production) {
      location = (<any>window.location);
    } else {
      location = "http://localhost:8080";
    }
    location = location.replace("http", "ws");
    this.socket = new WebSocket(location);
    this.socket.onopen = (e) => this.onOpen(e);
    this.socket.onmessage = (e) => this.onMessage(e);
    this.socket.onclose = (e) => this.onClose(e);
    this.socket.onerror = (e) => this.onError(e);
  }

  public sendAction(data: ClientAction) {
    this.socket.send(JSON.stringify(data));
  }

  public subscribeAction(a: Actions, callback: (data) => {}) {
    if (!this.subscribers.get(a)) {
      this.subscribers.set(a, [callback]);
    } else {
      this.subscribers.get(a).push(callback);
    }
  }

  private onOpen(e: Event) {
    console.log("Wss connection opened");
    this.error = false;
    this.connected = true;
  }

  private onMessage(e: MessageEvent) {
    let payload = JSON.parse(e.data);
    // for (let i of this.subscribers.get(payload.action) || []) {
    //   i(payload);
    // }
    this.gameState = payload;
    console.log(e);
  }

  private onClose(e: CloseEvent) {
    console.log("Connection closed");
    console.log(e);
    this.connected = false;
  }

  private onError(e: Event) {
    console.error("Connection failed");
    console.error(e);
    this.connected = false;
    this.error = true;
  }


}
