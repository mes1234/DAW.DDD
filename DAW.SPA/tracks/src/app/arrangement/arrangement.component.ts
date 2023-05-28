import { Component, OnInit } from '@angular/core';
import { CdkDragDrop, CdkDropList, CdkDrag, transferArrayItem, moveItemInArray } from '@angular/cdk/drag-drop';
import { Track } from "../models/track";
import { Clip } from "../models/clip";

@Component({
  selector: 'app-arrangement',
  templateUrl: './arrangement.component.html',
  styleUrls: ['./arrangement.component.less']
})
export class ArrangementComponent implements OnInit {

  Tracks: Track[] = [];

  constructor() {
  }

  ngOnInit(): void {

    this.Tracks.push(
      new Track(1, [
        new Clip('T1 Clip1'),
        new Clip('T1 Clip2'),
        new Clip('T1 Clip3'),
        new Clip('T1 Clip4'),
      ]));

    this.Tracks.push(
      new Track(2, [
        new Clip('T2 Clip1'),
        new Clip('T2 Clip2'),
        new Clip('T2 Clip3'),
        new Clip('T2 Clip4'),
      ]));

    this.Tracks.push(
      new Track(3, [
        new Clip('T3 Clip1'),
        new Clip('T3 Clip2'),
        new Clip('T3 Clip3'),
        new Clip('T3 Clip4'),
      ]));

    this.Tracks.push(
      new Track(4, [
        new Clip('T4 Clip1'),
        new Clip('T4 Clip2'),
        new Clip('T4 Clip3'),
        new Clip('T4 Clip4'),
      ]));
  }

  drop(event: CdkDragDrop<Clip[]>) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex,
      );
    }
  }

}
