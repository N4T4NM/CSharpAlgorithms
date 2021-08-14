using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CSharpAlgorithms.Pathfinding
{
    public static class AStar
    {
        public const int D = 10;
        public const int D2 = 14;

        public static List<Node> GetAdjescentNodes(Grid grid, Node node)
        {
            List<Node> nodes = new List<Node>();

            for (int x = node.Position.X - 1; x <= node.Position.X + 1; x++)
                for (int y = node.Position.Y - 1; y <= node.Position.Y + 1; y++)
                {
                    if (!grid.IsValid(x, y))
                        continue;

                    Node next = grid.GetNode(x, y);
                    if (next != node)
                        nodes.Add(next);
                }

            return nodes;
        }
        public static int GetDistance(Node start, Node goal)
        {
            int distanceX = Math.Abs(start.Position.X - goal.Position.X);
            int distanceY = Math.Abs(start.Position.Y - goal.Position.Y);

            if (distanceX > distanceY)
                return D2 * distanceY + D * (distanceX - distanceY);

            return Math.Abs(D2 * distanceX + D * (distanceX - distanceY));
        }
        public static void DrawPath(Node start, Node goal)
        {
            Node current = goal;
            while (current != start)
            {
                if (current != goal)
                    current.Type = NodeType.SelectedPath;
                current = current.Parent;
            }
        }
        public static Node[] GeneratePath(Node start, Node goal)
        {
            List<Node> result = new List<Node>();

            Node current = goal;
            while(current != start)
            {
                if (current != goal)
                    result.Add(current);
                current = current.Parent;
            }

            return result.ToArray();
        }
        public static Node[] Run(Grid grid)
        {
            Node start = grid.Nodes.Where(node => node.Value.Type == NodeType.Start).First().Value;
            Node goal = grid.Nodes.Where(node => node.Value.Type == NodeType.Goal).First().Value;

            List<Node> open = new List<Node>();
            HashSet<Node> visited = new HashSet<Node>();
            open.Add(start);

            while (open.Count > 0)
            {
                Node current = open[0];
                for (int i = 1; i < open.Count; i++)
                    if (open[i].FCost < current.FCost || open[i].FCost == current.FCost)
                        if (open[i].HCost < current.HCost)
                            current = open[i];

                open.Remove(current);
                visited.Add(current);

                if (current == goal)
                    return GeneratePath(start, goal);

                foreach(Node adjescent in GetAdjescentNodes(grid, current))
                {
                    if (adjescent.Type == NodeType.Wall || visited.Contains(adjescent))
                        continue;

                    int cost = current.GCost + GetDistance(current, adjescent);
                    if(cost < adjescent.GCost || !open.Contains(adjescent))
                    {
                        adjescent.GCost = cost;
                        adjescent.HCost = GetDistance(adjescent, goal);
                        adjescent.Parent = current;

                        if (!open.Contains(adjescent))
                            open.Add(adjescent);
                    }
                }
            }

            return new Node[0];
        }
    }

    public class Grid
    {
        public readonly int Width;
        public readonly int Height;
        public Dictionary<Point, Node> Nodes = new Dictionary<Point, Node>();

        public Grid(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    Nodes.Add(new Point(x, y), new Node(x, y));
        }

        public Node GetNode(int x, int y)
            => Nodes[new Point(x, y)];
        public NodeType GetNodeType(int x, int y)
            => GetNode(x, y).Type;
        public void SetNodeType(int x, int y, NodeType type)
            => GetNode(x, y).Type = type;
        public bool IsValid(int x, int y)
            => (x >= 0 && x < Width) && (y >= 0 && y < Height);
        public void Reset()
        {
            foreach (Node node in Nodes.Values)
            {
                if (node.Type == NodeType.SelectedPath)
                    node.Type = NodeType.Empty;

                node.Parent = null;
                node.GCost = int.MaxValue;
                node.HCost = int.MaxValue;
            }
        }
    }

    public class Node
    {
        public int GCost { get; set; } //Distance from starting node
        public int HCost { get; set; } //Heuristic distance from ending node
        public int FCost => GCost + HCost;

        public Node(int x, int y)
        {
            Position = new Point(x, y);
        }

        public NodeType Type;
        public Point Position;
        public Node Parent;
    }
    public enum NodeType
    {
        Empty = 0,
        Wall = 1,
        Start = 2,
        Goal = 3,
        SelectedPath = 4
    }
}
