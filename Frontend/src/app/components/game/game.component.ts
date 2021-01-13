import { Component, OnInit } from '@angular/core';
import { Actions, ClientAction } from 'src/app/classes/ClientAction';
import { ClipboardService } from 'src/app/services/clipboard.service';
import { ConnectionService } from 'src/app/services/connection.service';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent implements OnInit {
  constructor(
    public connection: ConnectionService,
    public clipboard: ClipboardService
  ) { }

  ngOnInit(): void { }

  public openNewTab(url: string) {
    window.open(url, "_blank");
  }

  public getLocation() {
    return window.location.href;
  }

  public getPlayers() {
    let players = [];
    for (let i in this.connection.gameState.oPlayers) {
      players.push(this.connection.gameState.oPlayers[i]);
    }
    return players;
  }

  public isPlayerTurn() {
    return this.connection.playerName === this.getCurrentPlayer();
  }

  public getCurrentPlayer() {
    return this.connection.gameState.oCurrentMessage.sPlayerName;
  }

  public canRoll() {
    if (this.connection.gameState.oCurrentMessage.sMessageType === Actions.ClientMoveFields) {
      return this.isPlayerTurn();
    }
    return false;
  }

  public getCurrentCoice() {
    return this.connection.gameState.oCurrentMessage.oChoice;
  }

  public getCurrentFieldText() {
    const position = this.connection.gameState.oPlayers[this.getCurrentPlayer()].shtBoardPosition;
    return this.connection.gameState.oBoard.oFields[position].sText;
  }

  public afterRoll(diceNumber) {
    var param = new ClientAction(Actions.ClientMoveFields);
    param.add("answer", diceNumber + "");
    this.connection.sendAction(param);
  }

  public executeAction(name: string = null) {
    var param = new ClientAction(Actions.ClientFieldAction);
    if (name) {
      param.add("answer", name);
    }
    this.connection.sendAction(param);
  }

  public getMessageType() {
    return this.connection.gameState.oCurrentMessage.sMessageType;
  }

  public isSkipMessage() {
    const type = this.getMessageType();
    return type === "NoFieldAction" || type === "NoSkipField" || type === "NoSnakeOrLadder" || type === "SnakeOrLadder" || type === "MissedEnd";
  }

  public isLobbyLeader() {
    return this.connection.gameState.sLobbyLeader === this.connection.playerName;
  }

  public kickPlayer(name: string) {
    var param = new ClientAction(Actions.ClientKick);
    param.add("name", name);
    this.connection.sendAction(param);
  }

}
