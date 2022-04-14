export class Field {
  constructor(
    public sText: string,
    public shtBoardPos: number
  ) { }
}

export class Board {
  constructor(
    public oFields: Field[]
  ) { }
}

export class Player {
  constructor(
    public sName: string,
    public shtBoardPosition: number,
    public lngPoints: number,
    public sColor: string,
    public shtOldBoardPosition: number
  ) { }
}

export class SnakeOrLadder {
  constructor(
    public shtSkipCost: number,
    public shtStartPoint: number,
    public shtEndPoint: number,
    public bSnake: boolean
  ) { }
}

export class Choice {
  constructor(
    public bCanSkip: boolean,
    public oChoices: string[],
    public sPlayer: string,
    public sSpecialType: string,
    public shtSkipCost: number
  ) { }
}

export class Message {
  constructor(
    public oChoice: Choice,
    public sMessageID: string,
    public sMessageType: string,
    public sPlayerName: string,
    public oAdditionalFields: string[]
  ) { }
}


export class GameState {
  constructor(
    public sGameId: string,
    public shtCurrentPlayer: number,
    public oPlayers: Map<string, Player>,
    public oBoard: Board,
    public intMaxFields: number,
    public intWidth: number,
    public intHeight: number,
    public oCurrentMessage: Message,
    public sLobbyLeader: string,
    public oSnakesAndLadders: SnakeOrLadder[]
  ) { }
}
