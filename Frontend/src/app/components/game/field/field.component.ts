import { AfterViewInit, Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { ConnectionService } from 'src/app/services/connection.service';

@Component({
  selector: 'app-field',
  templateUrl: './field.component.html',
  styleUrls: ['./field.component.css']
})
export class FieldComponent implements OnInit, AfterViewInit {
  @HostListener("window:resize", ["$event"])
	onResize(event) {
    this.draw();
  }

  @ViewChild("field") set field(e: ElementRef<HTMLTableElement>) {
    this._field = e.nativeElement;
    this.draw();
  };
  private _field : HTMLTableElement;

  @ViewChild("overlay") set overlay(e: ElementRef<HTMLCanvasElement>) {
    this._canvas = e.nativeElement;
    this.draw();
  };
  private _canvas : HTMLCanvasElement;

  constructor(
    private connection: ConnectionService
  ) { }

  ngOnInit(): void {
    this.draw();
  }

  ngAfterViewInit() {
    this.draw();
  }

  public getRows() {
    let rows = [];
    let w = this.connection.gameState.intWidth;
    let h = this.connection.gameState.intHeight;
    for (var y = 0; y < h; y++) {
      let row = this.connection.gameState.oBoard.oFields.slice(y * w, (y + 1) * w);
      if (y % 2 === 1) {
        row.reverse();
      }
      rows.push(row);
    }
    return rows;
  }

  public getPlayers() {
    let players = [];
    for (let i in this.connection.gameState.oPlayers) {
      players.push(this.connection.gameState.oPlayers[i]);
    }
    return players;
  }

  public getSnakes() {
    return this.connection.gameState.oSnakesAndLadders;
  }

  private drawArrow(ctx: CanvasRenderingContext2D, x1 = 0, y1 = 0, x2 = 0, y2 = 0) {
    var headlen = 10; //
    var dx = x2 - x1;
    var dy = y2 - y1;
    var angle = Math.atan2(dy, dx);
    ctx.lineWidth = 4;
    ctx.lineCap = "round";
    ctx.setLineDash([10, 10]);
    ctx.moveTo(x1, y1);
    ctx.lineTo(x2, y2);
    ctx.moveTo(x2, y2);
    ctx.lineTo(x2 - headlen * Math.cos(angle - Math.PI / 6), y2 - headlen * Math.sin(angle - Math.PI / 6));
    ctx.moveTo(x2, y2);
    ctx.lineTo(x2 - headlen * Math.cos(angle + Math.PI / 6), y2 - headlen * Math.sin(angle + Math.PI / 6));
  }

  private drawSnake(ctx: CanvasRenderingContext2D, x1 = 0, y1 = 0, x2 = 0, y2 = 0) {
    ctx.beginPath();
    ctx.strokeStyle = "#22AA3355";
    this.drawArrow(ctx, x1, y1, x2, y2);
    ctx.stroke();
  }

  private drawLadder(ctx: CanvasRenderingContext2D, x1 = 0, y1 = 0, x2 = 0, y2 = 0) {
    ctx.beginPath();
    ctx.strokeStyle = "#33333355";
    this.drawArrow(ctx, x1, y1, x2, y2);
    ctx.stroke();
  }

  draw() {
    if (!this._canvas || !this._field) { return; }
    const fieldRect = this._field.getBoundingClientRect();
    if (!fieldRect.width || !fieldRect.height) { return; }
    this._canvas.width = fieldRect.width;
    this._canvas.height = fieldRect.height;
    const ctx = this._canvas.getContext("2d");
    ctx.clearRect(0, 0, fieldRect.width, fieldRect.height);
    for (let i of this.connection.gameState.oSnakesAndLadders) {
      const startEl = document.getElementById("field" + i.shtStartPoint);
      const endEl = document.getElementById("field" + i.shtEndPoint);
      if (!startEl || !endEl) { continue; }
      const start = startEl.getBoundingClientRect();
      const end = endEl.getBoundingClientRect();
      const x1 = (start.left + start.right) / 2 - fieldRect.left;
      const y1 = start.bottom - fieldRect.top;
      const x2 = (end.left + end.right)  / 2 - fieldRect.left;
      const y2 = end.bottom - fieldRect.top;
      if (i.bSnake) {
        this.drawSnake(ctx, x1, y1, x2, y2);
      } else {
        this.drawLadder(ctx, x1, y1, x2, y2);
      }
    }
  }

}
