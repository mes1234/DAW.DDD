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

  width: number = 1550;
  height: number = 300;
  borderOffset: number = 1;

  className: string = "note"

  Notes: Note[] = [];
  Lines: number[] = [];
  NotesPositions: NotePosition[] = [];
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
      this.Notes.push(new Note(100, i * 150, 100, i + 1));
    }

    this.NotesPositions = this.calculateNotesPosition();
    this.Lines = this.calculateLinesPosition();
    this.Bars = this.calculateBarsPosition();
    this.Quants = this.calculateQuantsPosition();
    this.Beats = this.calculateBeatsPosition();
  }

  calculateNotesPosition(): NotePosition[] {
    let result: NotePosition[] = []

    let dh = this.clipLenghtService.NoteHeight_px;
    let msToPx = this.clipLenghtService.MsToPx;

    for (var note of this.Notes) {
      var noteH = dh;
      var position = new NotePosition(`${note.Pitch}.${note.Start}`, note.Start * msToPx, this.height - (note.Pitch - 1) * dh - noteH + 2 * this.borderOffset, note.Lenght * msToPx, noteH - 5 * this.borderOffset)
      result.push(position);
    }
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
    }
    $event.source._dragRef.reset();
  }
}
