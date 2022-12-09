using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell {
    public bool empty;
    public Vector2 pos;
    public Block block = null;
    public Cell[] adjacent;

    public Cell Initialize(Vector2 pos, bool empty) {
        this.pos = pos;
        this.empty = empty;
        adjacent = new Cell[4];
        return this;
    }

    public void SetAdjacent(Cell up, Cell right, Cell down, Cell left) {
        adjacent[0] = up;
        adjacent[1] = right;
        adjacent[2] = down;
        adjacent[3] = left;
    }
}
