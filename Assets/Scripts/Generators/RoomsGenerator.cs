using System.Collections.Generic;
using Behaviours;
using UnityEngine;

namespace Generators
{
    public class RoomsGenerator : MonoBehaviour
{
    private class Cell
    {
        public bool visited;
        public readonly bool[] status = new bool[4];
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int minPosition;
        public Vector2Int maxPosition;

        public bool obligatory;

        public int ProbabilityOfSpawning(int x, int y)
        {
            // 0 - cannot spawn 1 - can spawn 2 - HAS to spawn

            if (x>= minPosition.x && x<=maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
            {
                return obligatory ? 2 : 1;
            }

            return 0;
        }

    }

    public Vector2Int size;
    public int startPos;
    public Rule[] rooms;
    public Vector2 offset;

    private List<Cell> _board;

    // Start is called before the first frame update
    private void Start()
    {
        MazeGenerator();
    }

    private void GenerateDungeon()
    {

        for (var i = 0; i < size.x; i++)
        {
            for (var j = 0; j < size.y; j++)
            {
                var currentCell = _board[(i + j * size.x)];
                if (!currentCell.visited) continue;
                var randomRoom = -1;
                var availableRooms = new List<int>();

                for (var k = 0; k < rooms.Length; k++)
                {
                    var p = rooms[k].ProbabilityOfSpawning(i, j);

                    if(p == 2)
                    {
                        randomRoom = k;
                        break;
                    } else if (p == 1)
                    {
                        availableRooms.Add(k);
                    }
                }

                if(randomRoom == -1)
                {
                    randomRoom = availableRooms.Count > 0 ? availableRooms[Random.Range(0, availableRooms.Count)] : 0;
                }


                var newRoom = Instantiate(rooms[randomRoom].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                newRoom.UpdateRoom(currentCell.status);
                newRoom.name += " " + i + "-" + j;
            }
        }

    }

    private void MazeGenerator()
    {
        _board = new List<Cell>();

        for (var i = 0; i < size.x; i++)
        {
            for (var j = 0; j < size.y; j++)
            {
                _board.Add(new Cell());
            }
        }

        var currentCell = startPos;

        var path = new Stack<int>();

        var k = 0;

        while (k<1000)
        {
            k++;

            _board[currentCell].visited = true;

            if(currentCell == _board.Count - 1)
            {
                break;
            }

            //Check the cell's neighbors
            var neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0)
            {
                if (path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);

                var newCell = neighbors[Random.Range(0, neighbors.Count)];

                if (newCell > currentCell)
                {
                    //down or right
                    if (newCell - 1 == currentCell)
                    {
                        _board[currentCell].status[2] = true;
                        currentCell = newCell;
                        _board[currentCell].status[3] = true;
                    }
                    else
                    {
                        _board[currentCell].status[1] = true;
                        currentCell = newCell;
                        _board[currentCell].status[0] = true;
                    }
                }
                else
                {
                    //up or left
                    if (newCell + 1 == currentCell)
                    {
                        _board[currentCell].status[3] = true;
                        currentCell = newCell;
                        _board[currentCell].status[2] = true;
                    }
                    else
                    {
                        _board[currentCell].status[0] = true;
                        currentCell = newCell;
                        _board[currentCell].status[1] = true;
                    }
                }

            }

        }
        GenerateDungeon();
    }

    private List<int> CheckNeighbors(int cell)
    {
        var neighbors = new List<int>();

        //check up neighbor
        if (cell - size.x >= 0 && !_board[(cell-size.x)].visited)
        {
            neighbors.Add((cell - size.x));
        }

        //check down neighbor
        if (cell + size.x < _board.Count && !_board[(cell + size.x)].visited)
        {
            neighbors.Add((cell + size.x));
        }

        //check right neighbor
        if ((cell+1) % size.x != 0 && !_board[(cell +1)].visited)
        {
            neighbors.Add((cell +1));
        }

        //check left neighbor
        if (cell % size.x != 0 && !_board[(cell - 1)].visited)
        {
            neighbors.Add((cell -1));
        }

        return neighbors;
    }
}
}
