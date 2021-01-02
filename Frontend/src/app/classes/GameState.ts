export class Field {
  constructor(
    public sText: string,
    public shtBoardPos: number
  ) {}
}

export class Board {
  constructor(
    public oFields: Field[]
  ){ }
}

export class Player {
  constructor(
    public sName: string,
    public shtBoardPosition: number,
    public lngPoints: number
  ) {}
}

export class GameState {
  constructor(
      public sGameId: string,
      public shtCurrentPlayer: number,
      public oPlayers: Player[],
      public oBoard: Board
  ) {}
}
