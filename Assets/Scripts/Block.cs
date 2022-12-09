using DropIt;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Block : MonoBehaviour{
    public bool active, isNew, wasChecked = false;
    public BlockType type;
    public Cell cell = null;

    public Block Initialize(BlockType type, bool state, bool isNew) {
        this.type = type;
        this.active = state;
        this.isNew = isNew;
        return this;
    }

    public void ActivateBlock(bool state) {
        this.active = state;
    }

    public List<Block> Pop(bool start) {
        this.wasChecked = true;
        List<Block> toReturn = new List<Block>();

        foreach (Cell adj in this.cell.adjacent) {
            if (adj == null)
                continue;
            if (adj.block == null)
                continue;
            Block checking = adj.block;
            if (checking.wasChecked)
                continue;
            if (this.type == checking.type) {
                if (start)
                    toReturn.Add(this);
                toReturn.Add(checking);
                toReturn.AddRange(checking.Pop(false));
            }
        }
        wasChecked = false;
        return toReturn;
    }
}