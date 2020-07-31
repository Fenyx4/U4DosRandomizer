using System.Collections.Generic;

namespace U4DosRandomizer
{
    public class UltimaData
    {
        public List<Coordinate> LCB { get; set; }
        public List<Coordinate> Castles { get; set; }
        public List<Coordinate> Towns { get; set; }
        public List<Coordinate> Shrines { get; set; }
        public List<Coordinate> Moongates { get; set; }
        public List<Coordinate> Dungeons { get; set; }
        public List<Item> Items { get; set; }

        public UltimaData()
        {
            LCB = new List<Coordinate>();
            Castles = new List<Coordinate>();
            Towns = new List<Coordinate>();
            Shrines = new List<Coordinate>();
            Moongates = new List<Coordinate>();
            Dungeons = new List<Coordinate>();
            Items = new List<Item>();
        }
    }
}