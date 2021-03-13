using System;
using System.Collections.Generic;
using System.Text;

namespace U4DosRandomizer
{
    public class RiverNode
    {
        public ITile Coordinate { get; set; }
        public RiverNode Parent { get; set; }
        public List<RiverNode> Children { get; set; }
        public int depth { get; set; }
    }
}
