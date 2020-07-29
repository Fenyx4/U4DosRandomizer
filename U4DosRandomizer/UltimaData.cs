using System.Collections.Generic;

namespace U4DosRandomizer
{
    public class UltimaData
    {
        public List<Coordinate> LCB { get; set; }
        public List<Coordinate> Castles { get; set; }
        public List<Coordinate> Towns { get; set; }

        public UltimaData()
        {
            LCB = new List<Coordinate>();
            Castles = new List<Coordinate>();
            Towns = new List<Coordinate>();
        }
    }
}