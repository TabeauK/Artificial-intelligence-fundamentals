    using System;
using System.Collections.Generic;
using System.Text;

namespace MSI
{
    public class Piece
    {
        public int OnPieceId { get; set; }
        public int Id { get; set; }
        public bool On(Piece a) => a.Id == OnPieceId;
        public bool OnTable { get; set; }
        public bool Clear { get; set; }
        public Piece Clone()
        {
            return new Piece()
            {
                Id = Id,
                OnPieceId = OnPieceId,
                OnTable = OnTable,
                Clear = Clear,
            };
        }
    }
}
