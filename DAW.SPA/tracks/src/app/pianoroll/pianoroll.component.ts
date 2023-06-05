import { Component, OnInit } from '@angular/core';
import { Note, NotePosition } from '../models/note';
import { CdkDragEnd } from '@angular/cdk/drag-drop';
import { ClipPropertiesService } from '../services/clip-properties.service';

@Component({
  selector: 'app-pianoroll',
  templateUrl: './pianoroll.component.html',
  styleUrls: ['./pianoroll.component.less']
})
export class PianorollComponent implements OnInit {

  bars: number = 4;
  bpm: number = 100;
  beatsPerBar: number = 4;
  quantization: number = 8;

  width: number = 1050;
  height: number = 300;
  borderOffset: number = 1;

  className: string = "note"

  Notes: Map<number, Note> = new Map<number, Note>();
  Lines: number[] = [];
  NotesPositions: Map<number, NotePosition> = new Map<number, NotePosition>();
  Bars: number[] = [];
  Quants: number[] = [];
  Beats: number[] = [];

  constructor(private clipLenghtService: ClipPropertiesService) {

    clipLenghtService.Bars = this.bars;
    clipLenghtService.Bpm = this.bpm;
    clipLenghtService.BeatsPerBar = this.beatsPerBar;
    clipLenghtService.Quantization = this.quantization;
    clipLenghtService.ContainerWidthPx = this.width;
    clipLenghtService.ContainerHeightPx = this.height;
  }

  ngOnInit(): void {
    for (let i = 0; i < 16; i++) {
      this.Notes.set(i, new Note(100, i * 150, 1000, i + 1));
    }

    this.NotesPositions = this.calculateNotesPosition();
    this.Lines = this.calculateLinesPosition();
    this.Bars = this.calculateBarsPosition();
    this.Quants = this.calculateQuantsPosition();
    this.Beats = this.calculateBeatsPosition();
  }

  calculateNotesPosition(): Map<number, NotePosition> {
    let result: Map<number, NotePosition> = new Map<number, NotePosition>();

    let dh = this.clipLenghtService.NoteHeight_px;
    let msToPx = this.clipLenghtService.MsToPx;

    this.NotesPositions.values

    this.Notes.forEach((note: Note, key: number) => {
      var noteH = dh;
      var position = new NotePosition(`${note.Pitch}.${note.Start}`, note.Start * msToPx, this.height - (note.Pitch - 1) * dh - noteH + 2 * this.borderOffset, note.Lenght * msToPx, noteH - 5 * this.borderOffset)
      result.set(key, position);
    });

    return result;
  }

  calculateBarsPosition(): number[] {
    let result: number[] = []

    for (let i = 0; i < this.bars; i++) {
      result.push(this.clipLenghtService.BarLenght_px * i)
    }
    return result;
  }


  calculateBeatsPosition(): number[] {
    let result: number[] = []

    for (let i = 0; i < this.bars * 4; i++) {
      result.push(this.clipLenghtService.BeatLenght_px * i)
    }

    return result;
  }

  calculateQuantsPosition(): number[] {
    let result: number[] = []

    for (let i = 0; i < this.clipLenghtService.QunatsPerClip; i++) {
      result.push(this.clipLenghtService.QuantLenght_px * i)
    }

    return result;
  }


  calculateLinesPosition(): number[] {
    let result: number[] = []

    for (let i = 0; i < 16; i++) {
      result.push(this.clipLenghtService.NoteHeight_px * i)
    }

    return result;
  }


  doubleClick($event: MouseEvent) {

    let dh = this.clipLenghtService.NoteHeight_px;

    let x = $event.offsetX;
    let y = $event.offsetY;

    let pitchBase = Math.floor((this.height - y + dh + 2 * this.borderOffset) / dh - 1) + 1;
    let positionBase = this.clipLenghtService.SnapToQuant(x) / this.clipLenghtService.MsToPx;

    let siezeOfNotes = this.Notes.size;

    this.Notes.set(siezeOfNotes + 1, new Note(127, positionBase, this.clipLenghtService.QuantLenght_ms, pitchBase));

    this.NotesPositions = this.calculateNotesPosition();
  }

  dragEnd($event: CdkDragEnd) {

    let idOfElement = $event.source.element.nativeElement.id;
    let dh = this.clipLenghtService.NoteHeight_px;
    let element = document.getElementById(idOfElement);

    if (element != null) {
      let pitchBase = (this.height - parseFloat(element.style.top) + dh + 2 * this.borderOffset) / dh - 1;
      let pitchDiff = Math.floor(- $event.distance.y / dh);

      let pitchNew = pitchBase + pitchDiff
      if (pitchNew < 1) {
        pitchNew = 1
      };

      if (pitchNew > 16) {
        pitchNew = 16
      }

      element.style.backgroundColor = 'lime';
      let roundedY = this.height - (pitchNew - 1) * dh - dh + 2 * this.borderOffset
      let roundedX = this.clipLenghtService.SnapToQuant(parseFloat(element.style.left) + $event.distance.x);
      element.style.top = `${roundedY}px`;
      element.style.left = `${roundedX}px`;


      let currentNote = this.Notes.get(parseInt(idOfElement));

      this.Notes.set(parseInt(idOfElement), new Note(currentNote?.Velocity!, roundedX / this.clipLenghtService.MsToPx, currentNote?.Lenght!, pitchNew));

      this.NotesPositions = this.calculateNotesPosition();
    }

    $event.source._dragRef.reset();
  }
}
