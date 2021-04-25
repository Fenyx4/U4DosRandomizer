using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace U4DosRandomizer
{
    public class Region
    {
        public string Name { get; internal set; }
        public List<ITile> Tiles { get; internal set; }
        public Point Center { get; internal set; }
        public string RunicName { get; internal set; }
    }
}
