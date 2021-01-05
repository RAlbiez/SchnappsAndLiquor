import { ChangeDetectorRef, Component, HostListener, Input, OnInit } from '@angular/core';
import { SnakeOrLadder } from 'src/app/classes/GameState';

@Component({
  selector: 'app-snake-or-ladder',
  templateUrl: './snake-or-ladder.component.html',
  styleUrls: ['./snake-or-ladder.component.css']
})
export class SnakeOrLadderComponent implements OnInit {
	@HostListener("window:resize", ["$event"])
	onResize(event) {
    this.update();
	}

  @Input() snek: SnakeOrLadder;

  left = "";
  top = "";
  right = "";
  bottom = "";

  constructor(
    private cdk: ChangeDetectorRef
  ) {
    cdk.detach();
  }

  ngOnInit(): void {
    this.update();
  }
  update() {
    const start = document.getElementById("field" + this.snek.shtStartPoint).getBoundingClientRect();
    const end = document.getElementById("field" + this.snek.shtEndPoint).getBoundingClientRect();
    this.left = Math.min(start.left, end.left, start.right, end.right) + "px";
    this.top = Math.min(start.top, end.top, start.bottom, end.bottom) + "px";
    this.right = Math.max(start.right, end.right, start.left, end.left) + "px";
    this.bottom = Math.max(start.bottom, end.bottom, start.top, end.top) + "px";
    this.cdk.detectChanges();
  }

}
