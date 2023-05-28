import { Clip } from "./clip";

export class Track {
    public Clips: Clip[];
    public Id: number;

    constructor(id: number, clips: Clip[]) {
        this.Id = id;
        this.Clips = clips
    }

    public get ClassName() {
        return `example-list track${this.Id}`
    }
}