using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MSI
{
    public class Strips
    {
        public StripsState Start { get; set; }
        public StripsState End { get; set; }
        private int PieceCount { get; set; }

        private List<(int, int)> Actions { get; set; }
        public int GetMaxActionCode => 2 * PieceCount * PieceCount;

        //Przykład z wyjasnieniem:
        //20            //liczba elementów
        //              //pusta linia
        //false         //ARMEMPTY - początek definicji stanu początkowego
        //5             //id trzymanego elementu - HOLDING
        //CLEAR 3       //CLEAR(3) = true
        //ONTABLE 4     //ONTABLE(4) = true
        //CLEAR 3       //CLEAR(3) = true
        //ON 1 2        //ON(1,2) = true
        //CLEAR 1       //CLEAR(1) = true
        //CLEAR 10      //CLEAR(10) = true
        //              //pusta linia
        //true          //ARMEMPTY - początek definicji stanu końcowego
        //6             //id trzymanego elementu - HOLDING
        //CLEAR 3       //CLEAR(3) = true
        //ONTABLE 4     //ONTABLE(4) = true
        //CLEAR 3       //CLEAR(3) = true
        //ON 1 2        //ON(1,2) = true
        //CLEAR 1       //CLEAR(1) = true
        //CLEAR 10      //CLEAR(10) = true
        public void LoadFromFile(string filepath)
        {
            string[] lines = File.ReadAllLines(filepath);
            if (!int.TryParse(lines[0], out int count))
                throw new ArgumentException();
            PieceCount = count;
            Piece[] startPieces = new Piece[count];
            Piece[] endPieces = new Piece[count];
            for (int i = 0; i < PieceCount; i++)
            {
                startPieces[i] = new Piece()
                {
                    Id = i,
                    OnPieceId = -1,
                    Clear = false,
                    OnTable = false,
                };
                endPieces[i] = new Piece()
                {
                    Id = i,
                    OnPieceId = -1,
                    Clear = false,
                    OnTable = false,
                };
            }

            if (!bool.TryParse(lines[2], out bool startArmEmpty))
                throw new ArgumentException();
            if (!int.TryParse(lines[3], out int startHolding))
                throw new ArgumentException();
            int lineReader = 4;
            while (lines[lineReader] != "")
            {
                string[] condition = lines[lineReader].Split(' ');
                if (!int.TryParse(condition[1], out int id))
                    throw new ArgumentException();
                switch (condition[0])
                {
                    case "CLEAR":
                        {
                            startPieces[id].Clear = true;
                            break;
                        }
                    case "ONTABLE":
                        {
                            startPieces[id].OnTable = true;
                            break;
                        }
                    case "ON":
                        {
                            if (!int.TryParse(condition[2], out int id2))
                                throw new ArgumentException();
                            startPieces[id].OnPieceId = id2;
                            break;
                        }
                    default:
                        throw new ArgumentException();
                }
                lineReader++;
            }
            Start = new StripsState()
            {
                ArmEmpty = startArmEmpty,
                Holding = startHolding,
                State = startPieces.ToList(),
            };

            lineReader++;
            if (!bool.TryParse(lines[lineReader], out bool endArmEmpty))
                throw new ArgumentException();
            lineReader++;
            if (!int.TryParse(lines[lineReader], out int endHolding))
                throw new ArgumentException();
            lineReader++;
            while (lineReader < lines.Length)
            {
                string[] condition = lines[lineReader].Split(' ');
                if (!int.TryParse(condition[1], out int id))
                    throw new ArgumentException();
                switch (condition[0])
                {
                    case "CLEAR":
                        {
                            endPieces[id].Clear = true;
                            break;
                        }
                    case "ONTABLE":
                        {
                            endPieces[id].OnTable = true;
                            break;
                        }
                    case "ON":
                        {
                            if (!int.TryParse(condition[2], out int id2))
                                throw new ArgumentException();
                            endPieces[id].OnPieceId = id2;
                            break;
                        }
                    default:
                        throw new ArgumentException();
                }
                lineReader++;
            }
            End = new StripsState()
            {
                ArmEmpty = endArmEmpty,
                Holding = endHolding,
                State = endPieces.ToList(),
            };
            if (PieceCount > 1)
                setActions();
        }
        private void setActions()
        {
            Actions = new List<(int, int)>();
            for (int i = 2; i <= PieceCount; i++)
            {
                int k = i - 1;
                int s = i - 1;
                while (k > 0)
                {
                    Actions.Add((k - 1, s));
                    Actions.Add((s, --k));
                }
            }
            int a = 21;
        }

        public void SaveToFile(string filepath, List<int> chromosome)
        {
            using StreamWriter sw = new StreamWriter(filepath);
            foreach (int nr in chromosome)
            {
                int a = nr;
                if (a < PieceCount)
                {
                    sw.WriteLine($"PICKUP {a}");
                    continue;
                }
                a -= PieceCount;
                if (a < PieceCount)
                {
                    sw.WriteLine($"PUTDOWN {a}");
                    continue;
                }
                a -= PieceCount;
                if (a < PieceCount * (PieceCount - 1))
                {
                    sw.WriteLine($"STACK {Actions[a].Item1} {Actions[a].Item2}");
                    continue;
                }
                a -= PieceCount * (PieceCount - 1);
                if (a < PieceCount * (PieceCount - 1))
                {
                    sw.WriteLine($"UNSTACK {Actions[a].Item1} {Actions[a].Item2}");
                    continue;
                }
                throw new NotSupportedException();
            }
        }

        public double CompareStates(StripsState state1, StripsState state2)
        {
            int sum = 1; //Holding lub ArmEmpty
            foreach (var Piece in state2.State)
            {
                if (Piece.Id != state2.Holding)
                {
                    if (Piece.OnPieceId != -1) sum++;
                    if (Piece.Clear) sum++;
                    if (Piece.OnTable) sum++;
                }
            }
            int count = 0;
            if (state1.Holding == state2.Holding && state1.Holding != -1)
                count++;
            if (state1.ArmEmpty && state2.ArmEmpty)
                count++;
            foreach (var Piece in state1.State)
            {
                var Piece2 = state2.State.Find(x => x.Id == Piece.Id);
                if (Piece.Id != state1.Holding)
                {
                    if (Piece.OnPieceId == Piece2.OnPieceId && Piece2.OnPieceId != -1) count++;
                    if (Piece.Clear && Piece2.Clear) count++;
                    if (Piece.OnTable && Piece2.OnTable) count++;
                }
            }

            double tmp = count / (double)sum;
            return tmp >= 0.0 ? tmp : 0.0;
        }

        public StripsState ChromosomeToStateForward(List<int> chromosome)
        {
            StripsState tempState = Start.Clone();
            foreach (int nr in chromosome)
            {
                if (CanDoAction(nr, tempState, false))
                    tempState = DoAction(nr, tempState, false);
            }
            return tempState;
        }

        public StripsState ChromosomeToStateBackward(List<int> chromosome)
        {
            StripsState tempState = End.Clone();
            List<int> newChromosome = new List<int>(chromosome);
            newChromosome.Reverse();
            foreach (int nr in newChromosome)
            {
                if (CanDoAction(nr, tempState, true))
                    tempState = DoAction(nr, tempState, true);
            }
            return tempState;
        }

        public List<int> GetForwardExecutabilityVector(List<int> chromosome)
        {
            StripsState tempState = Start.Clone();
            int[] vector = new int[chromosome.Count];
            for (int i = 0; i < chromosome.Count; i++)
            {
                if (CanDoAction(chromosome[i], tempState, false))
                {
                    tempState = DoAction(chromosome[i], tempState, false);
                    vector[i] = 1;
                }
                else
                {
                    vector[i] = 0;
                }
            }
            return vector.ToList();
        }

        public List<int> GetBackwardExecutabilityVector(List<int> chromosome)
        {
            StripsState tempState = End.Clone();
            chromosome = new List<int>(chromosome);
            int[] vector = new int[chromosome.Count];
            List<int> newChromosome = new List<int>(chromosome);
            newChromosome.Reverse();
            for (int i = 0; i < chromosome.Count; i++)
            {
                if (CanDoAction(newChromosome[i], tempState, true))
                {
                    tempState = DoAction(newChromosome[i], tempState, true);
                    vector[i] = 1;
                }
                else
                {
                    vector[i] = 0;
                }
            }
            return vector.ToList();
        }

        public bool IsSolution(List<int> chromosome)
        {
            return GetForwardExecutabilityVector(chromosome).All(x => x == 1) && CompareStates(End, ChromosomeToStateForward(chromosome)) == 1.0;
        }

        private bool CanDoAction(int a, StripsState state, bool backward)
        {
            if (a < PieceCount)
                return !backward ? state.CanPickUp(state.State.Find(x => x.Id == a)) : state.CanPutDown(state.State.Find(x => x.Id == a));
            a -= PieceCount;
            if (a < PieceCount)
                return !backward ? state.CanPutDown(state.State.Find(x => x.Id == a)) : state.CanPickUp(state.State.Find(x => x.Id == a));
            a -= PieceCount;
            if (a < PieceCount * (PieceCount - 1))
            {
                return !backward ? state.CanStack(state.State.Find(x => x.Id == Actions[a].Item1), state.State.Find(x => x.Id == Actions[a].Item2)) : state.CanUnStack(state.State.Find(x => x.Id == Actions[a].Item1), state.State.Find(x => x.Id == Actions[a].Item2));
            }
            a -= PieceCount * (PieceCount - 1);
            if (a < PieceCount * (PieceCount - 1))
                return !backward ? state.CanUnStack(state.State.Find(x => x.Id == Actions[a].Item1), state.State.Find(x => x.Id == Actions[a].Item2)) : state.CanStack(state.State.Find(x => x.Id == Actions[a].Item1), state.State.Find(x => x.Id == Actions[a].Item2));
            throw new NotSupportedException();
        }


        private StripsState DoAction(int a, StripsState state, bool backward)
        {
            if (a < PieceCount)
                return !backward ? state.PickUp(state.State.Find(x => x.Id == a)) : state.PutDown(state.State.Find(x => x.Id == a));
            a -= PieceCount;
            if (a < PieceCount)
                return !backward ? state.PutDown(state.State.Find(x => x.Id == a)) : state.PickUp(state.State.Find(x => x.Id == a));
            a -= PieceCount;
            if (a < PieceCount * (PieceCount - 1))
                return !backward ? state.Stack(state.State.Find(x => x.Id == Actions[a].Item1), state.State.Find(x => x.Id == Actions[a].Item2)) : state.UnStack(state.State.Find(x => x.Id == Actions[a].Item1), state.State.Find(x => x.Id == Actions[a].Item2));
            a -= PieceCount * (PieceCount - 1);
            if (a < PieceCount * (PieceCount - 1))
                return !backward ? state.UnStack(state.State.Find(x => x.Id == Actions[a].Item1), state.State.Find(x => x.Id == Actions[a].Item2)) : state.Stack(state.State.Find(x => x.Id == Actions[a].Item1), state.State.Find(x => x.Id == Actions[a].Item2));
            throw new NotSupportedException();
        }
    }
}
