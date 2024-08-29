using System;

public struct Coords { // stores x+y coordinates
    public int x;
    public int y;
    public string spos;
    public Coords(string pos) {
        this.x = pos[0] - 'a';
        this.y = pos[1] - '0' - 1;
        spos = pos;
    }
    public Coords(int x, int y) {
        this.x = x;
        this.y = y;
        spos = (char)(x + 'a') + "" + (char)(y + '1');
    }
    public void setCoords(int x, int y) {
        this.x = x;
        this.y = y;
        spos = (char)(x + 'a') + "" + (char)(y + '1');
    }
    public void setCoords(string pos) {
        this.x = pos[0] - 'a';
        this.y = pos[1] - '0' - 1;
        spos = pos;
    }
    public override string ToString()
    {
        return "Chess form: " + spos + " Array form: (" + x + ", " + y + ")";
    }
    public bool Equals (Coords other) {
        return this.x == other.x && this.y == other.y;
    }
}