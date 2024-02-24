using System;
using System.Collections.Generic;
using System.Text;

namespace U4DosRandomizer
{
    public class River
    {
        public int RandomSeed { get; set; }
        public RiverNode Tree { get; internal set; }
        public Tuple<int, int> Direction { get; internal set; }

        // Prints the n-ary tree level wise
        public void LevelOrderTraversal(Action<RiverNode> actionOnNode)
        {
            if (Tree == null)
                return;

            // Standard level order traversal code
            // using queue
            Queue<RiverNode> q = new Queue<RiverNode>(); // Create a queue
            q.Enqueue(Tree); // Enqueue root 
            while (q.Count != 0)
            {
                int n = q.Count;

                // If this node has children
                while (n > 0)
                {
                    // Dequeue an item from queue
                    // and print it
                    RiverNode p = q.Peek();
                    q.Dequeue();
                    actionOnNode(p);

                    // Enqueue all children of 
                    // the dequeued item
                    for (int i = 0; i < p.Children.Count; i++)
                        q.Enqueue(p.Children[i]);
                    n--;
                }
            }
        }

        public int AddBridges(WorldMapGenerateMap map, int minDepth, int maxDepth)
        {
            int bridgeCount = 0;
            if (Tree == null)
            {
                return bridgeCount;
            }

            bridgeCount += AddBridges(Tree, map, minDepth, maxDepth);

            return bridgeCount;
        }

        private int AddBridges(RiverNode node, WorldMapGenerateMap map, int minDepth, int maxDepth)
        {
            int bridgeCount = 0;
            // Dont add it we aren't on shallow water (rare case of sailable river) or we are at the end of the river
            if (node.Coordinate.GetTile() != TileInfo.Shallow_Water || node.Children.Count == 0)
            {
                return bridgeCount;
            }

            //if(node.depth > minDepth)
            //{
            //    return bridgeCount;
            //}

            if (node.depth > minDepth && WorldMapGenerateMap.IsWalkableGround(map.GetCoordinate(node.Coordinate.X - 1, node.Coordinate.Y)) && WorldMapGenerateMap.IsWalkableGround(map.GetCoordinate(node.Coordinate.X + 1, node.Coordinate.Y)))
            {
                node.Coordinate.SetTile(TileInfo.Bridge);
                bridgeCount++;
            }
            else
            {
                foreach (var child in node.Children)
                {
                    bridgeCount += AddBridges(child, map, minDepth, maxDepth);
                }
            }

            return bridgeCount;
        }
    }
}
