export class Note {
    public Velocity: number;
    public Start: number; // defined in ms
    public Lenght: number; // defined in ms
    public Pitch: number;

    constructor(velocity: number, start: number, length: number, pitch: number) {
        this.Velocity = velocity;
        this.Lenght = length;
        this.Start = start;
        this.Pitch = pitch;
    }
}

export class NotePosition {
    public X: number;
    public Y: number;
    public Width: number;
    public Height: number;
    public Id: string

    constructor(id: string, x: number, y: number, width: number, height: number) {
        this.X = x;
        this.Y = y;
        this.Width = width;
        this.Height = height;
        this.Id = id;
    }
}