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
  className: string = "note"
  clipLenght: number = 850; //ms
  width: number = 1000;
  borderOffset: number = 2;
  height: number = 250;
  Notes: Note[] = [];
  NotesPositions: NotePosition[] = [];

  constructor() { }

  ngOnInit(): void {
    this.Notes.push(new Note(100, 0, 100, 1));
    this.Notes.push(new Note(100, 50, 100, 2));
    this.Notes.push(new Note(100, 100, 100, 3));
    this.Notes.push(new Note(100, 150, 100, 4));
    this.Notes.push(new Note(100, 200, 100, 5));
    this.Notes.push(new Note(100, 250, 100, 6));
    this.Notes.push(new Note(100, 300, 100, 7));
    this.Notes.push(new Note(100, 350, 100, 8));
    this.Notes.push(new Note(100, 400, 100, 9));
    this.Notes.push(new Note(100, 450, 100, 10));
    this.Notes.push(new Note(100, 500, 100, 11));
    this.Notes.push(new Note(100, 550, 100, 12));
    this.Notes.push(new Note(100, 600, 100, 13));
    this.Notes.push(new Note(100, 650, 100, 14));
    this.Notes.push(new Note(100, 700, 100, 15));
    this.Notes.push(new Note(100, 750, 100, 16));

    this.NotesPositions = this.calculateNotesPosition();
  }

  calculateNotesPosition(): NotePosition[] {
    let result: NotePosition[]

    let dh = this.height / 16
    let msToPx = this.width / this.clipLenght;

    result = []

    for (var note of this.Notes) {
      var noteH = dh;
      var position = new NotePosition(`${note.Pitch}.${note.Start}`, note.Start * msToPx, this.height - (note.Pitch - 1) * dh - noteH - this.borderOffset, note.Lenght * msToPx, noteH - 2 * this.borderOffset)
      result.push(position);
    }
    return result;
  }

  dragEnd($event: CdkDragEnd) {
    console.log("yello");
  }
}
