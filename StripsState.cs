using System;
using System.Collections.Generic;
using System.Text;

namespace MSI
{
    public class StripsState
    {
        public List<Piece> State { get; set; }
        public bool ArmEmpty { get; set; }
        public int Holding { get; set; }
        public bool CanStack(Piece a, Piece b) => Holding == a.Id && b.Clear;
        public bool CanUnStack(Piece a, Piece b) => a.On(b) && a.Clear && ArmEmpty;
        public bool CanPickUp(Piece a) => a.Clear && a.OnTable && ArmEmpty;
        public bool CanPutDown(Piece a) => a.Id == Holding;

        public StripsState Stack(Piece a, Piece b)
        {
            if (!CanStack(a, b))
                throw new NotSupportedException();
            List<Piece> newState = new List<Piece>();
            foreach (Piece p in State)
            {
                if (p.Id == a.Id)
                {
                    var temp = p.Clone();
                    //ADD && DELETE
                    temp.OnPieceId = b.Id;
                    temp.Clear = true;
                    newState.Add(temp);
                }
                else if (p.Id == b.Id)
                {
                    var temp = p.Clone();
                    //ADD && DELETE
                    temp.Clear = false;
                    newState.Add(temp);
                }
                else
                {
                    newState.Add(p.Clone());
                }
            }
            return new StripsState()
            {
                State = newState,
                //ADD && DELETE
                ArmEmpty = true,
                Holding = -1
            };
        }

        public StripsState UnStack(Piece a, Piece b)
        {
            if (!CanUnStack(a, b))
                throw new NotSupportedException();
            List<Piece> newState = new List<Piece>();
            foreach (Piece p in State)
            {
                if (p.Id == a.Id)
                {
                    var temp = p.Clone();
                    //ADD && DELETE
                    temp.OnPieceId = -1;
                    newState.Add(temp);
                }
                else if (p.Id == b.Id)
                {
                    var temp = p.Clone();
                    //ADD && DELETE
                    temp.Clear = true;
                    newState.Add(temp);
                }
                else
                {
                    newState.Add(p.Clone());
                }
            }
            return new StripsState()
            {
                State = newState,
                //ADD && DELETE
                ArmEmpty = false,
                Holding = a.Id,
            };
        }

        public StripsState PickUp(Piece a)
        {
            if (!CanPickUp(a))
                throw new NotSupportedException();
            List<Piece> newState = new List<Piece>();
            foreach (Piece p in State)
            {
                if (p.Id == a.Id)
                {
                    var temp = p.Clone();
                    //ADD && DELETE
                    temp.OnTable = false;
                    newState.Add(temp);
                }
                else
                {
                    newState.Add(p.Clone());
                }
            }
            return new StripsState()
            {
                State = newState,
                //ADD && DELETE
                ArmEmpty = false,
                Holding = a.Id,
            };
        }

        public StripsState PutDown(Piece a)
        {
            if (!CanPutDown(a))
                throw new NotSupportedException();
            List<Piece> newState = new List<Piece>();
            foreach (Piece p in State)
            {
                if (p.Id == a.Id)
                {
                    var temp = p.Clone();
                    //ADD && DELETE
                    temp.OnTable = true;
                    temp.Clear = true;
                    newState.Add(temp);
                }
                else
                {
                    newState.Add(p.Clone());
                }
            }
            return new StripsState()
            {
                State = newState,
                //ADD && DELETE
                ArmEmpty = true,
                Holding = -1,
            };
        }

        public StripsState Clone()
        {
            List<Piece> newState = new List<Piece>();
            foreach (Piece p in State)
            {
                newState.Add(p.Clone());
            }
            return new StripsState()
            {
                State = newState,
                ArmEmpty = ArmEmpty,
                Holding = Holding,
            };
        }
    }
}
