import { ChangeDetectorRef, Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-dice',
  templateUrl: './dice.component.html',
  styleUrls: ['./dice.component.css']
})
export class DiceComponent implements OnInit {
  diceNumber = 1;
  rolling = false;

  @Output() rolled: EventEmitter<number> = new EventEmitter();
  @Output() startRoll: EventEmitter<number> = new EventEmitter();

  constructor(
    private cdr: ChangeDetectorRef
  ) {
    cdr.detach();
  }

  ngOnInit(): void {
    this.cdr.detectChanges();
  }

  public roll() {
    if (this.rolling) { return; }
    this.startRoll.emit();
    this.rolling = true;
    this.cdr.detach();
    const iterations = 23
    var i = 0;
    const singleRoll = () => {
      this.diceNumber = Math.floor(Math.random() * 6 + 1);
      this.cdr.detectChanges();
      if (++i < iterations) {
        setTimeout(() => { singleRoll(); }, (i / iterations) * 220);
        return;
      }
      this.rolling = false;
      this.rolled.emit(this.diceNumber);
    };
    singleRoll();
  }

  public coinFlip() {
    return Math.random() > 0.5;
  }

}
