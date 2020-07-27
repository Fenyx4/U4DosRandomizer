using System.Collections.Generic;

namespace U4DosRandomizer
{
    public class UltimaData
    {
        public List<Location> LCB { get; set; }
        public List<Location> Castles { get; set; }
        public List<Location> Towns { get; set; }

        public UltimaData()
        {
            LCB = new List<Location>();
            Castles = new List<Location>();
            Towns = new List<Location>();
        }
    }
}