import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { GameComponent } from './components/game/game.component';
import { SplashScreenComponent } from './components/splash-screen/splash-screen.component';
import { DiceComponent } from './components/dice/dice.component';
import { SnakeOrLadderComponent } from './components/snake-or-ladder/snake-or-ladder.component';

@NgModule({
  declarations: [
    AppComponent,
    GameComponent,
    SplashScreenComponent,
    DiceComponent,
    SnakeOrLadderComponent
  ],
  imports: [
    BrowserModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
