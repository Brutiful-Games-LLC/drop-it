using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DropIt;
using System.Linq;
using System;

public class GameController : MonoBehaviour {
    [SerializeField] [Range(6, 10)] private int _rowCount = 10;
    [SerializeField] [Range(4, 6)] private int _variations = 4;
    [SerializeField] private GameObject[] _theme;
    [SerializeField] private GameObject _cell;
    [SerializeField] private float _cellSize = 3.2f;
    [SerializeField][Range(5f, 30f)] private float _targetSpeed = 10f;

    private float _new, _newL, _newR;

    private float _speed;
    private Cell[][] _grid;
    private List<Block> _blocks, _dirBlocks;
    private Block _newLeft, _newRight = null;
    private int _ctr = 0;
    private bool _spawned = false, _swiped = false;

    void Start() {
        _blocks = new List<Block>();
        _dirBlocks = new List<Block>();
        _speed = _targetSpeed;
        _new = _cellSize * _rowCount + 0.75f;
        _newL = _cellSize;
        _newR = _cellSize * 2f;
        InitializeCells();
    }

    void Update() {
        if (!_spawned) {
            _spawned = true;
            _swiped = false;
            _speed = _targetSpeed;
            BlockType leftType = (BlockType)UnityEngine.Random.Range(0, _variations - 1);
            BlockType rightType = (BlockType)UnityEngine.Random.Range(0, _variations - 1);
            _newLeft = Instantiate(_theme[(int)leftType], new Vector3(_newL, _new, 0), Quaternion.identity).AddComponent<Block>().Initialize(leftType, true, true);
            _newRight = Instantiate(_theme[(int)rightType], new Vector3(_newR, _new, 0), Quaternion.identity).AddComponent<Block>().Initialize(rightType, true, true);
            _blocks.Add(_newLeft);
            _blocks.Add(_newRight);
            _newLeft.name = "Block " + _ctr++;
            _newRight.name = "Block " + _ctr++;
            DirectBlocks();
        }
        MoveBlocks();

        if (Input.GetKeyDown(KeyCode.A))
            Swipe(3);
        if (Input.GetKeyDown(KeyCode.D))
            Swipe(1);
        if (Input.GetKeyDown(KeyCode.S))
            Swipe(2);
    }

    private void InitializeCells() {
        _grid = new Cell[4][];
        for (int i = 0; i < 4; i++) {
            _grid[i] = new Cell[_rowCount];
            for (int j = 0; j < _rowCount; j++)
                _grid[i][j] = new Cell().Initialize(new Vector2(_cellSize * i, _cellSize * j), true);
        }

        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < _rowCount; j++) {
                _grid[i][j].SetAdjacent(
                    j != _rowCount - 1 ? _grid[i][j + 1] : null,
                    i != 3 ? _grid[i + 1][j] : null,
                    j != 0 ? _grid[i][j - 1] : null,
                    i != 0 ? _grid[i - 1][j] : null
                );
            }
        }
    }

    private void DirectBlocks() {
        if (_spawned && !_dirBlocks.Contains(_newLeft) && !_dirBlocks.Contains(_newRight)) {
            Cell leftCell = null, rightCell = null;
            if (_newLeft != null) {
                foreach (Cell cell in _grid[GetCol(_newLeft)]) {
                    if (cell.pos.y > _newLeft.transform.position.y)
                        break;
                    if (cell.empty && leftCell == null)
                        leftCell = cell;
                    else if (!cell.empty)
                        leftCell = null;
                }
            }
            if (_newRight != null) {
                foreach (Cell cell in _grid[GetCol(_newRight)]) {
                    if (cell.empty && rightCell == null) {
                        rightCell = cell;
                    }
                    else if (!cell.empty)
                        rightCell = null;
                }
            }

            if (leftCell.pos.y > rightCell.pos.y)
                rightCell = leftCell.adjacent[1];
            else if (leftCell.pos.y < rightCell.pos.y)
                leftCell = rightCell.adjacent[3];
            
            leftCell.block = _newLeft;
            rightCell.block = _newRight;
            leftCell.empty = false;
            rightCell.empty = false;
            _newLeft.cell = leftCell;
            _newRight.cell = rightCell;
            _dirBlocks.Add(_newLeft);
            _dirBlocks.Add(_newRight);
        }

        foreach (Block block in _blocks.Except(_dirBlocks)) {
            if (block.active) {
                Cell target = null;
                foreach (Cell cell in _grid[GetCol(block)]) {
                    if (cell.pos.y > block.transform.position.y)
                        break;
                    if ((cell == block.cell || cell.empty) && target == null)
                        target = cell;
                    else if (cell != block.cell && !cell.empty && target != null)
                        target = null;
                }
                if (target == block.cell) {
                    block.active = false;
                    continue;
                }
                target.block = block;
                target.empty = false;
                block.cell.block = null;
                block.cell.empty = true;
                block.cell = target;
                _dirBlocks.Add(block);
            }
        }
    }

    private void MoveBlocks() {
        List<Block> remove = new List<Block>();
        foreach (Block block in _dirBlocks) {
            Vector2 blockPos = (Vector2)block.transform.position, cellPos = block.cell.pos;
            if (blockPos == cellPos) {
                remove.Add(block);
                block.active = false;
            }
            else
                block.transform.position = Vector2.MoveTowards(blockPos, cellPos, _speed * Time.deltaTime);
        }
        foreach (Block block in remove) {
            _dirBlocks.Remove(block);
            TryPop(block);
        }
        if (_spawned && (!_dirBlocks.Contains(_newLeft) || !_dirBlocks.Contains(_newRight))) {
            _newLeft.isNew = false;
            _newRight.isNew = false;
            _newLeft = null;
            _newRight = null;
            _spawned = false;
        }   
    }

    private void Swipe(int dir) {
        if (_swiped)
            return;
        switch (dir) {
            case 1:
                _dirBlocks.Remove(_newLeft);
                _dirBlocks.Remove(_newRight);
                _newLeft.cell.block = null;
                _newRight.cell.block = null;
                _newLeft.cell.empty = true;
                _newRight.cell.empty = true;
                _newLeft.cell = null;
                _newRight.cell = null;
                _newLeft.transform.position = new Vector2(_cellSize * 2, _newLeft.transform.position.y);
                _newRight.transform.position = new Vector2(_cellSize * 3, _newRight.transform.position.y);
                DirectBlocks();
                _speed *= 3f;
                break;

            case 2:
                _speed *= 3f;
                break;

            case 3:
                _dirBlocks.Remove(_newLeft);
                _dirBlocks.Remove(_newRight);
                _newLeft.cell.block = null;
                _newRight.cell.block = null;
                _newLeft.cell.empty = true;
                _newRight.cell.empty = true;
                _newLeft.cell = null;
                _newRight.cell = null;
                _newLeft.transform.position = new Vector2(0, _newLeft.transform.position.y);
                _newRight.transform.position = new Vector2(_cellSize, _newRight.transform.position.y);
                DirectBlocks();
                _speed *= 3f;
                break;
        }
        _swiped = true;
    }

    private int GetCol(Block block) {
        if (block == null)
            return -1;
        return (int)(block.transform.position.x / 3.2f);
    }

    private void TryPop(Block block) {
        List<Block> toPop = block.Pop(true);
        if (toPop.Count == 0)
            return;
        if (toPop.Count == 2) {
            if (toPop[0].isNew == toPop[1].isNew)
                return;
        }
        foreach (Block popBlock in toPop) {
            popBlock.cell.block = null;
            popBlock.cell.empty = true;
            if (popBlock.isNew) {
                foreach (Cell adjCell in popBlock.cell.adjacent.ToList()) {
                    if (adjCell == null)
                        continue;
                    if (adjCell.block == null)
                        continue;
                    if (toPop.Contains(adjCell.block))
                        continue;
                    if (adjCell.block.isNew) {
                        adjCell.block.active = true;
                        break;
                    }
                }
            }
            if (popBlock.cell.adjacent[0] != null) {
                if (popBlock.cell.adjacent[0].block != null) {
                    popBlock.cell.adjacent[0].block.active = true;
                }
            }
            _blocks.Remove(popBlock);
            Destroy(popBlock.gameObject);
        }
    }
}
