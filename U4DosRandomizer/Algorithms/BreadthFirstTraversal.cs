using System.Collections.Generic;

namespace U4DosRandomizer.Algorithms
{
    public class BreadthFirstTraversal
    {
        public delegate List<ITile> GetNeighbors(ITile coord);

        public static void BreadthFirst(ITile startTile, IsNodeValid actionPerTile, GetNeighbors getNeighbors)
        {
            var queue = new Queue<ITile>();
            queue.Enqueue(startTile);
            HashSet<ITile> closedset = new HashSet<ITile>(); // The set of nodes visited
            closedset.Add(startTile);
            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();

                closedset.Add(cell);
                actionPerTile(cell);
                var neighbors = getNeighbors(cell);
                foreach (var neighbor in neighbors)
                {
                    if (!closedset.Contains(neighbor))
                    {
                        closedset.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }
    }
}
