import { Component, OnInit } from '@angular/core';
import { Actions, ClientAction } from 'src/app/classes/ClientAction';
import { ConnectionService } from 'src/app/services/connection.service';

@Component({
  selector: 'app-splash-screen',
  templateUrl: './splash-screen.component.html',
  styleUrls: ['./splash-screen.component.css']
})
export class SplashScreenComponent implements OnInit {
  public gameCode = "";
  public playerName = "unbenannter"
  constructor(
    private connection: ConnectionService
    ) { }

  ngOnInit(): void {
  }

  createLobby() {
    var param = new ClientAction(Actions.ClientCreateLobby);
    param.add("name", this.playerName);
    console.log(param);
    this.connection.sendAction(param);
  }

  joinGame() {
    var param = new ClientAction(Actions.ClientJoinGame);
    param.add("name", this.playerName);
    param.add("lobbyId", this.gameCode);
    this.connection.sendAction(param);
  }

}
