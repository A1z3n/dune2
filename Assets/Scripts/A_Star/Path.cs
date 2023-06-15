using System;
using System.Collections.Generic;
using SuperTiled2Unity;

namespace AI.A_Star
{
    /// <summary>
    /// Reusable A* path finder.
    /// </summary>
    public class Path : IPath
    {
        private const int MaxNeighbours = 8;
        private readonly PathNode[] neighbours = new PathNode[MaxNeighbours];

        private readonly int maxSteps;
        private readonly IBinaryHeap<Vector2Int, PathNode> frontier;
        private readonly HashSet<Vector2Int> ignoredPositions;
        private readonly List<Vector2Int> output;
        private readonly IDictionary<Vector2Int, Vector2Int> links;

        /// <summary>
        /// Creation of new path finder.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Path(int maxSteps = int.MaxValue, int initialCapacity = 0)
        {
            if (maxSteps <= 0) throw new ArgumentOutOfRangeException(nameof(maxSteps));
            if (initialCapacity < 0) throw new ArgumentOutOfRangeException(nameof(initialCapacity));

            this.maxSteps = maxSteps;
            frontier = new BinaryHeap<Vector2Int, PathNode>(a => a.Position, initialCapacity);
            ignoredPositions = new HashSet<Vector2Int>(initialCapacity);
            output = new List<Vector2Int>(initialCapacity);
            links = new Dictionary<Vector2Int, Vector2Int>(initialCapacity);
        }

        /// <summary>
        /// Calculate a new path between two points.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Calculate(Vector2Int start, Vector2Int target, IReadOnlyCollection<Vector2Int> obstacles, out IReadOnlyCollection<Vector2Int> path)
        {
            if (obstacles == null) throw new ArgumentNullException(nameof(obstacles));

            if (!GenerateNodes(start, target, obstacles))
            {
                path = Array.Empty<Vector2Int>();
                return false;
            }

            output.Clear();
            output.Add(target);

            while (links.TryGetValue(target, out target)) output.Add(target);
            path = output;
            return true;
        }

        public bool CalculateNext(Vector2Int start, Vector2Int target, IReadOnlyCollection<Vector2Int> obstacles, out Vector2Int next)
        {
            if (obstacles == null) throw new ArgumentNullException(nameof(obstacles));
            //if ((target - start).DistanceEstimate() >= 2) {
            //    foreach (var o in obstacles) {
            //        if (target.X == o.X && target.Y == o.Y) {
            //            //target = FindNearest(start, target, obstacles);
            //            break;
            //        }
            //    }
            //}

            if (!GenerateNodes(start, target, obstacles)) {
                next = new Vector2Int();
                return false;
            }
            output.Clear();
            output.Add(target);

            while (links.TryGetValue(target, out target)) output.Add(target);
            output.Reverse();
            if (output.IsEmpty())
            {
                next = new Vector2Int();
                return false;
            }
            if (output[0].X == start.X && output[0].Y == start.Y)
                output.RemoveAt(0);
            if (output.IsEmpty()) {
                next = new Vector2Int();
                return false;
            }
            next = output[0];
            return true;
        }

        private bool GenerateNodes(Vector2Int start, Vector2Int target, IReadOnlyCollection<Vector2Int> obstacles)
        {
            frontier.Clear();
            ignoredPositions.Clear();
            links.Clear();

            frontier.Enqueue(new PathNode(start, target, 0));
            ignoredPositions.UnionWith(obstacles);
            var step = 0;
            while (frontier.Count > 0 && step++ <= maxSteps)
            {
                PathNode current = frontier.Dequeue();
                ignoredPositions.Add(current.Position);

                if (current.Position.Equals(target)) return true;

                GenerateFrontierNodes(current, target);
            }

            // All nodes analyzed - no path detected.
            return false;
        }

        private Vector2Int FindNearest(Vector2Int start, Vector2Int target, IReadOnlyCollection<Vector2Int> obstacles) {
            Vector2Int result = new Vector2Int();

            Dictionary<Vector2Int, int> paths = new Dictionary<Vector2Int, int>();
            for(int rad = 1; rad<3; rad++){
                for (int x = target.X-rad; x <= target.X+rad; x++) {
                    for (int y = target.Y-rad; y <= target.Y+rad; y++) {
                        bool found = false;
                        Vector2Int pos = new Vector2Int(x, y);
                        foreach (var o in obstacles) {
                            if (o.X == x && o.Y == y) {
                                found = true;
                                break;
                            }
                        }

                        if (found) continue;
                        IReadOnlyCollection<AI.A_Star.Vector2Int> path = new List<AI.A_Star.Vector2Int>();
                        if (Calculate(start, pos, obstacles,out path)) {
                            paths[pos] = path.Count;
                        }
                    }
                }

                if (paths.IsEmpty()) continue;
                int shortest = 999999;
                Vector2Int shortestVector;
                foreach (var p in paths) {
                    if (shortest > p.Value) {
                        shortest = p.Value;
                        result = p.Key;
                    }
                }

                return result;

            }
            return result;
        }

        private void GenerateFrontierNodes(PathNode parent, Vector2Int target)
        {
            neighbours.Fill(parent, target);
            foreach (PathNode newNode in neighbours)
            {
                // Position is already checked or occupied by an obstacle.
                if (ignoredPositions.Contains(newNode.Position)) continue;

                // Node is not present in queue.
                if (!frontier.TryGet(newNode.Position, out PathNode existingNode))
                {
                    frontier.Enqueue(newNode);
                    links[newNode.Position] = parent.Position;
                }

                // Node is present in queue and new optimal path is detected.
                else if (newNode.TraverseDistance < existingNode.TraverseDistance)
                {
                    frontier.Modify(newNode);
                    links[newNode.Position] = parent.Position;
                }
            }
        }
    }
}