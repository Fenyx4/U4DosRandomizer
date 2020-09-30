using System.Collections.Generic;
using U4DosRandomizer.BlueRajaPriorityQueue;

namespace U4DosRandomizer
{
	public delegate bool IsNodeValid(ITile coord);
	public delegate float NodeHuersticValue(ITile coord, IsNodeValid matchesGoal);

	public class Search
    {
		public static List<Tile> GetPath(int sizeX, int sizeY, ITile startNode, IsNodeValid matchesGoal, IsNodeValid validNode, NodeHuersticValue heuristic = null)
        {
			return GetPath(sizeX, sizeY, new List<ITile> { startNode }, matchesGoal, validNode, heuristic);
        }

		public static List<Tile> GetPath(int sizeX, int sizeY, List<ITile> startNodes, IsNodeValid matchesGoal, IsNodeValid validNode, NodeHuersticValue heuristic = null)
		{
			if (heuristic == null)
			{
				heuristic = delegate (ITile n, IsNodeValid m) {
					return 0;
				};
			}

			HashSet<ITile> closedset = new HashSet<ITile>(); // The set of nodes already evaluated
			HashSet<ITile> openset = new HashSet<ITile>(); // The set of tentative nodes to be evaluated, initially containing the start node
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

			HeapPriorityQueue<PriorityQueueCoordinate<Tile>> f_score = new HeapPriorityQueue<PriorityQueueCoordinate<Tile>>(sizeX * sizeY);
			foreach (Tile node in g_score.Keys)
			{
				f_score.Enqueue(new PriorityQueueCoordinate<Tile>(node), g_score[node] + heuristic(node, matchesGoal));
			}


			while (openset.Count > 0)
			{
				// Find queued index with lowest score
				Tile current = f_score.Dequeue().GetCoord();

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
						f_score.Enqueue(new PriorityQueueCoordinate<Tile>(neighbor), g_score[neighbor] + heuristic(neighbor, matchesGoal));
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

        public static HashSet<Tile> GetSuccessfulPaths(int sizeX, int sizeY, ITile startNode, HashSet<Tile> goals, IsNodeValid validNode)
        {
			var results = new HashSet<Tile>();
			HashSet<ITile> closedset = new HashSet<ITile>(); // The set of nodes already evaluated
			HashSet<ITile> openset = new HashSet<ITile>(); // The set of tentative nodes to be evaluated, initially containing the start node

			openset.Add(startNode);

			Dictionary<Tile, Tile> came_from = new Dictionary<Tile, Tile>(); // The map of navigated nodes.

			Dictionary<ITile, int> g_score = new Dictionary<ITile, int>();

			g_score.Add(startNode, 0);

			HeapPriorityQueue<PriorityQueueCoordinate<Tile>> f_score = new HeapPriorityQueue<PriorityQueueCoordinate<Tile>>(sizeX * sizeY);
			foreach (Tile node in g_score.Keys)
			{
				f_score.Enqueue(new PriorityQueueCoordinate<Tile>(node), g_score[node]);
			}


			while (openset.Count > 0)
			{
				// Find queued index with lowest score
				Tile current = f_score.Dequeue().GetCoord();

				if (goals.Contains(current))
				{
					results.Add(current);
					if(results.Equals(goals))
                    {
						return results;
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
						f_score.Enqueue(new PriorityQueueCoordinate<Tile>(neighbor), g_score[neighbor]);
						if (!openset.Contains(neighbor))
						{
							openset.Add(neighbor);
						}
					}
				}
			}

			//No path found
			return results;
		}
    }
}
