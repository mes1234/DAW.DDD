import { TestBed } from '@angular/core/testing';

import { ClipPropertiesService } from './clip-properties.service';

describe('ClipPropertiesService', () => {
  let service: ClipPropertiesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ClipPropertiesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('Should calculate correct clip lenght', () => {
    // arrange
    service.Bars = 4;
    service.BeatsPerBar = 4;
    service.Bpm = 100;
    // act

    const clipLenght = service.ClipLength_ms;

    // assert
    expect(clipLenght).toBe(9600); //ms
  });

  it('Should calculate correct beat lenght', () => {
    // arrange
    service.Bpm = 100;
    // act

    const beatLenght = service.BeatLenght_ms;

    // assert
    expect(beatLenght).toBe(600); //ms
  });

  it('Should calculate correct 16th note length', () => {
    // arrange
    service.Bpm = 100;
    service.Quantization = 16;
    // act

    const beatLenght = service.QuantLenght_ms;

    // assert
    expect(beatLenght).toBe(150); //ms
  });

  it('Should calculate correct 8th note length', () => {
    // arrange
    service.Bpm = 100;
    service.Quantization = 8;
    // act

    const beatLenght = service.QuantLenght_ms;

    // assert
    expect(beatLenght).toBe(300); //ms
  });

  it('Should calculate correct bars px possition', () => {
    //arrange
    service.Bpm = 100;
    service.ContainerWidthPx = 1000;
    service.Bars = 4;
    //act

    const barsLenght = service.BarLenght_px

    //assert

    expect(barsLenght).toBe(250); //px
  })
});
