import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ClipPropertiesService {

  public Bpm: number = 100;
  public Bars: number = 4;
  public BeatsPerBar: number = 4;
  public Quantization: number = 16;
  public ContainerWidthPx: number = 1000;
  public ContainerHeightPx: number = 1000;
  public NotesCount: number = 16;

  constructor() { }

  public get MsToPx() {
    return this.ContainerWidthPx / this.ClipLength_ms;
  }

  public get QuantLenght_ms(): number {
    return this.BeatLenght_ms / this.Quantization * 4;
  }

  public get QuantLenght_px(): number {
    return this.QuantLenght_ms * this.MsToPx;
  }

  public get BeatLenght_ms(): number {
    return 60. / this.Bpm * 1000
  }

  public get BeatLenght_px(): number {
    return this.BeatLenght_ms * this.MsToPx;
  }

  public get BarLenght_ms(): number {
    return this.BeatLenght_ms * this.BeatsPerBar
  }

  public get BarLenght_px(): number {
    return this.BarLenght_ms * this.MsToPx
  }

  public get ClipLength_ms(): number {
    return this.BarLenght_ms * this.Bars
  }

  public get ClipLength_px(): number {
    return this.ClipLength_px * this.MsToPx
  }

  public get NoteHeight_px(): number {
    return this.ContainerHeightPx / this.NotesCount;
  }

  public get QunatsPerClip(): number {
    return this.Bars * this.BeatsPerBar * this.Quantization;
  }

  public SnapToQuant(position_px: number): number {
    const quantIntervalPx = this.QuantLenght_px;

    return position_px - (position_px % quantIntervalPx);

  }

}
