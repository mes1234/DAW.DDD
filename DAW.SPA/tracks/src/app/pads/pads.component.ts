import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { SoundEngineService } from '../services/sound-engine.service';

@Component({
  selector: 'app-pads',
  templateUrl: './pads.component.html',
  styleUrls: ['./pads.component.less']
})
export class PadsComponent implements OnInit {

  constructor(private soundEngine: SoundEngineService) { }

  ngOnInit(): void {

    this.soundEngine.FetchSound("5850b158-10b4-47c5-9e1b-a5c979b3f25c");
  }


  play($event: Event): void {
    let elementId: string = ($event.target as Element).id;
    this.soundEngine.PlaySound(elementId);
  }
}
