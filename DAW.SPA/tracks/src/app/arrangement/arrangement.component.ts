import { Component, OnInit } from '@angular/core';
import { CdkDragDrop, CdkDropList, CdkDrag, transferArrayItem, moveItemInArray } from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-arrangement',
  templateUrl: './arrangement.component.html',
  styleUrls: ['./arrangement.component.less']
})
export class ArrangementComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

  track1Clips = [
    'T1 Clip1',
    'T1 Clip2',
    'T1 Clip3',
    'T1 Clip4',
  ];

  track2Clips = [
    'T2 Clip5',
    'T2 Clip6',
    'T2 Clip7',
    'T2 Clip8',
  ];

  track3Clips = [
    'Clip9',
    'Clip10',
    'Clip11',
    'Clip12',
  ];

  track4Clips = [
    'Clip13',
    'Clip14',
    'Clip15',
    'Clip16',
  ];

  // drop(event: CdkDragDrop<string[]>) {
  //   moveItemInArray(this.track1Clips, event.previousIndex, event.currentIndex);
  // }

  drop(event: CdkDragDrop<string[]>) {
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
