import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PianorollComponent } from './pianoroll.component';

describe('PianorollComponent', () => {
  let component: PianorollComponent;
  let fixture: ComponentFixture<PianorollComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PianorollComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PianorollComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
