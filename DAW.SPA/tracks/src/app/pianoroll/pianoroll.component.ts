import { Component, OnInit } from '@angular/core';
import { Note, NotePosition } from '../models/note';
import { CdkDragEnd } from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-pianoroll',
  templateUrl: './pianoroll.component.html',
  styleUrls: ['./pianoroll.component.less']
})
export class PianorollComponent implements OnInit {

  private draggingElement: any;
  clipLenght: number = 1000; //ms
  width: number = 1000;
  borderOffset: number = 2;
  height: number = 200;
  Notes: Note[] = [];
  NotesPositions: NotePosition[] = [];

  constructor() { }

  ngOnInit(): void {
    this.Notes.push(new Note(100, 0, 100, 1));
    this.Notes.push(new Note(100, 500, 100, 2));
    this.Notes.push(new Note(100, 700, 100, 3));
    this.Notes.push(new Note(100, 900, 100, 4));
    this.Notes.push(new Note(100, 950, 100, 5));
    this.Notes.push(new Note(100, 400, 100, 16));

    this.NotesPositions = this.calculateNotesPosition();
  }

  calculateNotesPosition(): NotePosition[] {
    let result: NotePosition[]

    let dh = (this.height - this.borderOffset) / 16
    let msToPx = this.width / this.clipLenght;

    result = []

    for (var note of this.Notes) {
      var position = new NotePosition(note.Start * msToPx, this.height - note.Pitch * dh, note.Lenght * msToPx, dh - 2 * this.borderOffset)
      result.push(position);
    }
    return result;
  }

  dragEnd($event: CdkDragEnd) {
    console.log("yello");
  }
}
