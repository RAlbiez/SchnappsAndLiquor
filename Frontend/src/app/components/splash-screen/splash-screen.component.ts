import { Component, OnInit } from '@angular/core';
import { Actions, ClientAction } from 'src/app/classes/ClientAction';
import { ConnectionService } from 'src/app/services/connection.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-splash-screen',
  templateUrl: './splash-screen.component.html',
  styleUrls: ['./splash-screen.component.css']
})
export class SplashScreenComponent implements OnInit {
  constructor(
    public connection: ConnectionService
  ) { }

  changeName = false;

  ngOnInit(): void {
    // if (!environment.production) {
    //   this.createLobby();
    //   return;
    // }
    var lobbyId = this.connection.getIdFromUrl();
    if (lobbyId) {
      this.connection.gameCode = lobbyId;
    }
  }

  createLobby() {
    var param = new ClientAction(Actions.ClientCreateLobby);
    param.add("name", this.connection.playerName);
    this.connection.sendAction(param);
  }

  joinGame() {
    var param = new ClientAction(Actions.ClientJoinGame);
    param.add("name", this.connection.playerName);
    param.add("lobbyId", this.connection.gameCode);
    this.connection.sendAction(param);
  }

}
