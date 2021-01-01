export enum Actions {
    ClientCreateLobby = "ClientCreateLobby",
    ClientJoinGame = "ClientJoinGame",
    ClientRollDice = "ClientRollDice",
    ServerJoinGame = "ServerJoinGame"
}

class KeyVal {
    constructor(
        public Key: string,
        public Value: string
    ) { }
}

export class ClientAction {
    public Parameters: KeyVal[] = [];

    constructor(
        public Type: Actions
    ) {}

    public add(key: string, value: string) {
        this.Parameters.push(new KeyVal(key, value));
    }
}
