using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LifeOfGame {
    internal struct Inputs {
        public readonly long LivingInGen0; // r
        public readonly long GenerationCount; // t
        public readonly long BlockX;
        public readonly long BlockY;
    
        public readonly List<LineState> LineStates;

        public Inputs(long livingInGen0, long generationCount, long blockX, long blockY, List<LineState> lineStates) {
            this.LivingInGen0 = livingInGen0;
            this.GenerationCount = generationCount;
            this.BlockX = blockX;
            this.BlockY = blockY;
            this.LineStates = lineStates;
        }
    }

    internal struct LineState {
        public readonly long Y;
        public readonly long[] X;

        public LineState(long y, long[] x) {
            this.Y = y;
            this.X = x;
        }
    }

    internal struct Output {
        private readonly long _cellsAliveCount;
        private readonly bool[,] _neighborStates;

        public Output(long cellsAliveCount, bool[,] neighborStates) {
            this._cellsAliveCount = cellsAliveCount;
            this._neighborStates = neighborStates;
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(this._cellsAliveCount.ToString());
            for (int y = 0; y < 5; y++) {
                StringBuilder lineBuilder = new StringBuilder();
            
                for (int x = 0; x < 5; x++) {
                    bool isAlive = this._neighborStates[x, y];
                    lineBuilder.Append(isAlive ? "0" : ".");
                }

                builder.AppendLine(lineBuilder.ToString());
            }

            return builder.ToString();
        }
    }

    internal class Cell {
        private readonly long _x;
        private readonly long _y;

        public Cell(long x, long y) {
            this._x = x;
            this._y = y;
        }

        public override int GetHashCode() {
            return (int) (this._x ^ this._y);
        }

        public override bool Equals(object obj) {
            Cell otherCell = (Cell)obj;
            if (otherCell == null) {
                return false;
            }

            return this._x == otherCell._x && this._y == otherCell._y;
        }

        public Cell GetNeighbor(Direction direction) {
            switch (direction) {
                case Direction.Tl:
                    return new Cell(this._x - 1, this._y - 1);
                case Direction.T:
                    return new Cell(this._x, this._y - 1);
                case Direction.Tr:
                    return new Cell(this._x + 1, this._y - 1);
                case Direction.R:
                    return new Cell(this._x + 1, this._y);
                case Direction.Br:
                    return new Cell(this._x + 1, this._y + 1);
                case Direction.B:
                    return new Cell(this._x, this._y + 1);
                case Direction.Bl:
                    return new Cell(this._x - 1, this._y + 1);
                case Direction.L:
                    return new Cell(this._x - 1, this._y);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
        
        public IEnumerable<Cell> GetAllNeighbors() {
            return new[] {
                GetNeighbor(Direction.Tl),
                GetNeighbor(Direction.T),
                GetNeighbor(Direction.Tr),
                GetNeighbor(Direction.R),
                GetNeighbor(Direction.Br),
                GetNeighbor(Direction.B),
                GetNeighbor(Direction.Bl),
                GetNeighbor(Direction.L),
            };
        }
    }

    internal enum Direction {
        Tl,
        T,
        Tr,
        R,
        Br,
        B,
        Bl,
        L,
    }

    internal static class Program {

        public static void Main() {
            Inputs inputs = ParseStdin();
            Output output = CalculateOutput(inputs);
            PrintOutput(output);
        }

        private static Output CalculateOutput(Inputs input) {
            IEnumerable<LineState> lineStates = MergeLineStates(input.LineStates);

            HashSet<Cell> cells = new HashSet<Cell>((int)input.LivingInGen0);
            
            // Populate the lists
            foreach (LineState state in lineStates) {
                foreach (long x in state.X) {
                    cells.Add(new Cell(x, state.Y));
                }
            }

            List<Cell> cellsToRemove = new List<Cell>();
            List<Cell> cellsToAdd = new List<Cell>();
            for (int generation = 0; generation < input.GenerationCount; generation++) {
                foreach (Cell aliveCell in cells) {
                    (bool alive, List<Cell> revived) = WillCellLive(cells, aliveCell);
                    if (!alive) {
                        cellsToRemove.Add(aliveCell);
                    }
                    
                    cellsToAdd.AddRange(revived);
                }
            }

            foreach (Cell dead in cellsToRemove) {
                cells.Remove(dead);
            }
            
            cells.UnionWith(cellsToAdd);

            long cellsAliveCount = cells.Count;
            Cell targetCell = new Cell(input.BlockX, input.BlockY);
            bool[,] targetCellSurroundings = new bool[,] {
                {
                    cells.Contains(targetCell.GetNeighbor(Direction.Tl).GetNeighbor(Direction.Tl)),
                    cells.Contains(targetCell.GetNeighbor(Direction.Tl).GetNeighbor(Direction.T)),
                    cells.Contains(targetCell.GetNeighbor(Direction.T).GetNeighbor(Direction.T)),
                    cells.Contains(targetCell.GetNeighbor(Direction.Tr).GetNeighbor(Direction.T)),
                    cells.Contains(targetCell.GetNeighbor(Direction.Tr).GetNeighbor(Direction.Tr)),
                },
                {
                    cells.Contains(targetCell.GetNeighbor(Direction.Tl).GetNeighbor(Direction.L)),  
                    cells.Contains(targetCell.GetNeighbor(Direction.Tl)),  
                    cells.Contains(targetCell.GetNeighbor(Direction.T)),  
                    cells.Contains(targetCell.GetNeighbor(Direction.Tr)),  
                    cells.Contains(targetCell.GetNeighbor(Direction.Tr).GetNeighbor(Direction.R)),  
                },
                {
                    cells.Contains(targetCell.GetNeighbor(Direction.L).GetNeighbor(Direction.L)),
                    cells.Contains(targetCell.GetNeighbor(Direction.L)),
                    cells.Contains(targetCell),
                    cells.Contains(targetCell.GetNeighbor(Direction.R)),
                    cells.Contains(targetCell.GetNeighbor(Direction.L).GetNeighbor(Direction.R)),
                },
                {
                    cells.Contains(targetCell.GetNeighbor(Direction.Bl).GetNeighbor(Direction.L)),  
                    cells.Contains(targetCell.GetNeighbor(Direction.Bl)),  
                    cells.Contains(targetCell.GetNeighbor(Direction.B)),  
                    cells.Contains(targetCell.GetNeighbor(Direction.Br)),  
                    cells.Contains(targetCell.GetNeighbor(Direction.Br).GetNeighbor(Direction.R)), 
                },
                {
                    cells.Contains(targetCell.GetNeighbor(Direction.Bl).GetNeighbor(Direction.Bl)),
                    cells.Contains(targetCell.GetNeighbor(Direction.Bl).GetNeighbor(Direction.B)),
                    cells.Contains(targetCell.GetNeighbor(Direction.B).GetNeighbor(Direction.B)),
                    cells.Contains(targetCell.GetNeighbor(Direction.Br).GetNeighbor(Direction.B)),
                    cells.Contains(targetCell.GetNeighbor(Direction.Br).GetNeighbor(Direction.Br)),
                }
            };

            return new Output(cellsAliveCount, targetCellSurroundings);
        }

        private static (bool, List<Cell>) WillCellLive(ICollection<Cell> aliveCellsCurrentGeneration, Cell cell) {
            int neighborsAlive = 0;
            List<Cell> deadNeighbors = new List<Cell>();
            
            foreach (Cell neighbor in cell.GetAllNeighbors()) {
                if (aliveCellsCurrentGeneration.Contains(neighbor)) {
                    neighborsAlive++;
                } else {
                    deadNeighbors.Add(neighbor);
                }
            }

            bool returnValue = neighborsAlive == 2 || neighborsAlive == 3;

            List<Cell> revivedCells = new List<Cell>();
            foreach (Cell deadCell in deadNeighbors) {
                neighborsAlive = 0;
                
                foreach (Cell neighbor in deadCell.GetAllNeighbors()) {
                    if (!returnValue && neighbor != cell) {
                        if (aliveCellsCurrentGeneration.Contains(neighbor)) {
                            neighborsAlive++;
                        }   
                    }
                }

                if (neighborsAlive == 3) {
                    revivedCells.Add(deadCell);
                }
            }

            return (returnValue, revivedCells);
        }

        private static IEnumerable<LineState> MergeLineStates(IEnumerable<LineState> originalStates) { 
            Dictionary<long, List<long>> yLines = new Dictionary<long, List<long>>();

            foreach (LineState originalState in originalStates) {
                if (yLines.ContainsKey(originalState.Y)) {
                    List<long> xValues = yLines[originalState.Y];
                    xValues.AddRange(originalState.X);
                    yLines[originalState.Y] = xValues;
                } else {
                    yLines[originalState.Y] = originalState.X.ToList();
                }
            }

            List<LineState> newStates = new List<LineState>();
            foreach(KeyValuePair<long, List<long>> entry in yLines) {
                newStates.Add(new LineState(entry.Key, entry.Value.ToArray()));
            }

            return newStates;
        }

        private static Inputs ParseStdin() {
            string stdin = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(stdin)) {
                Environment.Exit(0);
            }
            
            string[] parts = stdin.Split(' ');

            long livingInGen0 = long.Parse(parts[0].Trim());
            long generationCount = long.Parse(parts[1].Trim());
            long blockX = long.Parse(parts[3].Trim());
            long blockY = long.Parse(parts[4].Trim());

            List<LineState> lineStates = new List<LineState>();
            do {
                stdin = Console.ReadLine();
                if (string.IsNullOrEmpty(stdin)) {
                    break;
                }

                parts = stdin.Split(' ');
                if (parts.Length == 0) {
                    break;
                }

                long y = 0;
                List<long> xBuffer = new List<long>();
            
                for (int i = 0; i < parts.Length; i++) {
                    string part = parts[i];
                    if (string.IsNullOrWhiteSpace(part)) {
                        continue;
                    }

                    if (i == 0) {
                        y = long.Parse(part.Trim());
                    } else {
                        xBuffer.Add(long.Parse(part.Trim()));
                    }
                }

                LineState lineState = new LineState(y, xBuffer.ToArray());
                lineStates.Add(lineState);
            } while (true);

            return new Inputs(livingInGen0, generationCount, blockX, blockY, lineStates);
        }

        private static void PrintOutput(Output output) {
            Console.WriteLine(output.ToString());
        }
    }
}