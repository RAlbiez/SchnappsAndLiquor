import { Component, OnInit } from '@angular/core';
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

  ngOnInit(): void {
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

  public getRows() {
    let rows = [];
    let w = this.connection.gameState.intWidth;
    let h = this.connection.gameState.intHeight;
    for (var y = 0; y < h; y++) {
      rows.push(this.connection.gameState.oBoard.oFields.slice(y * w, (y + 1) * w));
    }
    return rows;
  }

}
