import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment'
import { Players, loaded, start, ToneAudioBuffers, Sampler, Part, Transport } from 'tone';

@Injectable({
  providedIn: 'root'
})
export class SoundEngineService {

  address: string = environment.apiAddress
  samples: ToneAudioBuffers = new ToneAudioBuffers();
  players: Players = new Players();
  sampler: Sampler = new Sampler();

  constructor() { }

  /**
   * FetchSound
   */
  public FetchSound(id: string): void {

    this.players = new Players({
      urls: {
        A1: `https://localhost:7267/sounds/${id}/1`,
        A2: `https://localhost:7267/sounds/${id}/2`,
        A3: `https://localhost:7267/sounds/${id}/3`,
        A4: `https://localhost:7267/sounds/${id}/4`,
        A5: `https://localhost:7267/sounds/${id}/5`,
        A6: `https://localhost:7267/sounds/${id}/6`,
        A7: `https://localhost:7267/sounds/${id}/7`,
        A8: `https://localhost:7267/sounds/${id}/8`,
        A9: `https://localhost:7267/sounds/${id}/9`,
        A10: `https://localhost:7267/sounds/${id}/10`,
        A11: `https://localhost:7267/sounds/${id}/11`,
        A12: `https://localhost:7267/sounds/${id}/12`,
        A13: `https://localhost:7267/sounds/${id}/13`,
        A14: `https://localhost:7267/sounds/${id}/14`,
        A15: `https://localhost:7267/sounds/${id}/15`,
        A16: `https://localhost:7267/sounds/${id}/16`,
      }
    });

    this.sampler = new Sampler({
      urls: {
        C1: `https://localhost:7267/sounds/${id}/1`,
        D1: `https://localhost:7267/sounds/${id}/2`,
        E1: `https://localhost:7267/sounds/${id}/3`,
        F1: `https://localhost:7267/sounds/${id}/4`,
        G1: `https://localhost:7267/sounds/${id}/5`,
        A1: `https://localhost:7267/sounds/${id}/6`,
        B1: `https://localhost:7267/sounds/${id}/7`,
        C2: `https://localhost:7267/sounds/${id}/8`,
        D2: `https://localhost:7267/sounds/${id}/9`,
        E3: `https://localhost:7267/sounds/${id}/10`,
        F3: `https://localhost:7267/sounds/${id}/11`,
        G3: `https://localhost:7267/sounds/${id}/12`,
        A3: `https://localhost:7267/sounds/${id}/13`,
        B3: `https://localhost:7267/sounds/${id}/14`,
        C4: `https://localhost:7267/sounds/${id}/15`,
        D4: `https://localhost:7267/sounds/${id}/16`,
      }
    });
  }

  public PlaySound(pitch: string): void {
    start();
    // this.sampler.triggerRelease(pitch).toDestination;
    //  this.sampler.triggerAttackRelease(pitch, 100.5).toDestination();
    // const player = this.players.player(pitch).toDestination();
    // player.stop();
    // player.start();

    // const part = new Part(((time, note) => {
    //   // the notes given as the second element in the array
    //   // will be passed in as the second argument
    //   this.sampler.triggerAttackRelease(pitch, 100.5);
    // }), [[0, "C2"], ["0:2", "C3"], ["0:3:2", "G2"]]);
    Transport.position = 0;
    const part = new Part(((time, value) => {
      // the value is an object which contains both the note and the velocity
      this.sampler.triggerAttackRelease(value.note, "8n", time, value.velocity).toDestination();
    }),
      [
        { time: 0, note: "C1", velocity: 0.9 },
        { time: 1, note: "D1", velocity: 0.5 },
        { time: 2, note: "E1", velocity: 0.5 },
        { time: 3, note: "F1", velocity: 0.5 }
      ]).start(0);

    Transport.start()



  }


}
