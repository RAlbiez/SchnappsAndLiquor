import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { GameComponent } from './components/game/game.component';
import { SplashScreenComponent } from './components/splash-screen/splash-screen.component';
import { DiceComponent } from './components/game/dice/dice.component';
import { FieldComponent } from './components/game/field/field.component';

@NgModule({
  declarations: [
    AppComponent,
    GameComponent,
    SplashScreenComponent,
    DiceComponent,
    FieldComponent
  ],
  imports: [
    BrowserModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
