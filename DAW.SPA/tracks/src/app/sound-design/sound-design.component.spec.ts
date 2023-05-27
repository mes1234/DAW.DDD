import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SoundDesignComponent } from './sound-design.component';

describe('SoundDesignComponent', () => {
  let component: SoundDesignComponent;
  let fixture: ComponentFixture<SoundDesignComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SoundDesignComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SoundDesignComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
