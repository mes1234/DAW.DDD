import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { MatGridListModule } from '@angular/material/grid-list';
import { MatButtonModule } from '@angular/material/button';
import { NgFor } from '@angular/common';
import { DragDropModule } from '@angular/cdk/drag-drop';

import { SoundDesignComponent } from './sound-design/sound-design.component';
import { PadsComponent } from './pads/pads.component';
import { ArrangementComponent } from './arrangement/arrangement.component';

@NgModule({
  declarations: [
    AppComponent,
    SoundDesignComponent,
    PadsComponent,
    ArrangementComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatGridListModule,
    MatButtonModule,
    DragDropModule,
    NgFor
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
