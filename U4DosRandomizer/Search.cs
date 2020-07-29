using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using U4DosRandomizer.BlueRajaPriorityQueue;

namespace U4DosRandomizer
{
	public delegate bool IsNodeValid(Coordinate coord);
	public delegate float NodeHuersticValue(Coordinate coord, IsNodeValid matchesGoal);

	public class Search
    {
		public static List<Coordinate> GetPath(int sizeX, int sizeY, List<Coordinate> startNodes, IsNodeValid matchesGoal, IsNodeValid validNode, NodeHuersticValue heuristic = null)
		{
			if (heuristic == null)
			{
				heuristic = delegate (Coordinate n, IsNodeValid m) {
					return 0;
				};
			}

			HashSet<Coordinate> closedset = new HashSet<Coordinate>(); // The set of nodes already evaluated
			HashSet<Coordinate> openset = new HashSet<Coordinate>(); // The set of tentative nodes to be evaluated, initially containing the start node
			foreach (var node in startNodes)
			{
				openset.Add(node);
			}
			Dictionary<Coordinate, Coordinate> came_from = new Dictionary<Coordinate, Coordinate>(); // The map of navigated nodes.

			Dictionary<Coordinate, int> g_score = new Dictionary<Coordinate, int>();
			foreach (Coordinate node in startNodes)
			{
				g_score.Add(node, 0);
			}

			HeapPriorityQueue<PriorityQueueCoordinate<Coordinate>> f_score = new HeapPriorityQueue<PriorityQueueCoordinate<Coordinate>>(sizeX * sizeY);
			foreach (Coordinate node in g_score.Keys)
			{
				f_score.Enqueue(new PriorityQueueCoordinate<Coordinate>(node), g_score[node] + heuristic(node, matchesGoal));
			}


			while (openset.Count > 0)
			{
				// Find queued index with lowest score
				Coordinate current = f_score.Dequeue().GetCoord();

				if (matchesGoal(current))
				{
					// Walk backwards through the list and build the path
					List<Coordinate> path = new List<Coordinate>();
					while (true)
					{
						path.Insert(0, current);
						if (startNodes.Contains(current))
						{
							return path;
						}

						current = came_from[current];
					}
				}

				openset.Remove(current);
				closedset.Add(current);
				foreach (Coordinate neighbor in current.NeighborCoordinates())
				{
					if (closedset.Contains(neighbor))
					{
						continue;
					}

					if (validNode != null)
					{
						if (!validNode(neighbor))
						{
							continue;
						}
					}

					int bestTileScore = (g_score.ContainsKey(neighbor) ? g_score[neighbor] : 0);
					int tentative_g_score = bestTileScore + 1;

					if (!openset.Contains(neighbor) || tentative_g_score < g_score[neighbor])
					{
						came_from[neighbor] = current;
						g_score[neighbor] = tentative_g_score;
						f_score.Enqueue(new PriorityQueueCoordinate<Coordinate>(neighbor), g_score[neighbor] + heuristic(neighbor, matchesGoal));
						if (!openset.Contains(neighbor))
						{
							openset.Add(neighbor);
						}
					}
				}
			}

			//No path found
			return new List<Coordinate>();
		}

        
    }
}
