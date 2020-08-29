using System.Collections.Generic;
using Priority_Queue;

namespace U4DosRandomizer
{
	public delegate bool IsNodeValid(Tile coord);
	public delegate float NodeHuersticValue(Tile coord, IsNodeValid matchesGoal);

	public class Search
    {
		public static List<Tile> GetPath(int sizeX, int sizeY, Tile startNode, IsNodeValid matchesGoal, IsNodeValid validNode, NodeHuersticValue heuristic = null)
        {
			return GetPath(sizeX, sizeY, new List<Tile> { startNode }, matchesGoal, validNode, heuristic);
        }

		public static List<Tile> GetPath(int sizeX, int sizeY, List<Tile> startNodes, IsNodeValid matchesGoal, IsNodeValid validNode, NodeHuersticValue heuristic = null)
		{
			if (heuristic == null)
			{
				heuristic = delegate (Tile n, IsNodeValid m) {
					return 0;
				};
			}

			HashSet<Tile> closedset = new HashSet<Tile>(); // The set of nodes already evaluated
			HashSet<Tile> openset = new HashSet<Tile>(); // The set of tentative nodes to be evaluated, initially containing the start node
			foreach (var node in startNodes)
			{
				openset.Add(node);
			}
			Dictionary<Tile, Tile> came_from = new Dictionary<Tile, Tile>(); // The map of navigated nodes.

			Dictionary<Tile, int> g_score = new Dictionary<Tile, int>();
			foreach (Tile node in startNodes)
			{
				g_score.Add(node, 0);
			}

			FastPriorityQueue<Tile> f_score = new FastPriorityQueue<Tile>(sizeX * sizeY);
			foreach (Tile node in g_score.Keys)
			{
				f_score.Enqueue(node, g_score[node] + heuristic(node, matchesGoal));
			}


			while (openset.Count > 0)
			{
				// Find queued index with lowest score
				Tile current = f_score.Dequeue();

				if (matchesGoal(current))
				{
					// Walk backwards through the list and build the path
					List<Tile> path = new List<Tile>();
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
				foreach (Tile neighbor in current.NeighborCoordinates())
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
						f_score.Enqueue(neighbor, g_score[neighbor] + heuristic(neighbor, matchesGoal));
						if (!openset.Contains(neighbor))
						{
							openset.Add(neighbor);
						}
					}
				}
			}

			//No path found
			return new List<Tile>();
		}

        
    }
}
