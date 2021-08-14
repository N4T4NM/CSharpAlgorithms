using CSharpAlgorithms.Pathfinding;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Examples.UI
{
    public class AStarPanel : Panel
    {
        public Grid Grid = new Grid(10, 10);
        public AStarPanel() : base()
        {

        }

        static Pen pen = new Pen(Color.Black, 1);
        static SolidBrush[] brushes =
        {
            new SolidBrush(DefaultBackColor),
            new SolidBrush(Color.DarkGray),
            new SolidBrush(Color.Yellow),
            new SolidBrush(Color.GreenYellow),
            new SolidBrush(Color.Aqua)
        };

        private void DrawGrid(Graphics g)
        {
            int offsetX = this.Width / Grid.Width;
            int offsetY = this.Height / Grid.Height;

            for (int x = 1; x < Grid.Width; x++)
                g.DrawLine(pen, new Point(x * offsetX, 0), new Point(x * offsetX, this.Height));

            for (int y = 1; y < Grid.Height; y++)
                g.DrawLine(pen, new Point(0, y * offsetY), new Point(this.Width, y * offsetY));
        }
        private void DrawNode(int x, int y, Graphics g)
        {
            int offsetX = this.Width / Grid.Width;
            int offsetY = this.Height / Grid.Height;

            Node Node = Grid.GetNode(x, y);
            Rectangle rect = new Rectangle((x * offsetX) + 1, (y * offsetY) + 1, offsetX - 1, offsetY - 1);
            g.FillRectangle(brushes[(int)Node.Type], rect);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DrawGrid(e.Graphics);
            for (int x = 0; x < Grid.Width; x++)
                for (int y = 0; y < Grid.Height; y++) { DrawNode(x, y, e.Graphics); }

            base.OnPaint(e);
        }

        protected Point SnapToGrid(Point point)
        {
            int offsetX = this.Width / Grid.Width;
            int offsetY = this.Height / Grid.Height;

            return new Point(point.X / offsetX, point.Y / offsetY);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            Grid.Reset();

            Point pos = SnapToGrid(e.Location);
            if (!Grid.IsValid(pos.X, pos.Y))
                return;
            int current = (int)Grid.GetNodeType(pos.X, pos.Y);

            if (e.Button == MouseButtons.Left)
                current++;
            else if (e.Button == MouseButtons.Right)
                current--;

            if (current > 3)
                current = 0;
            else if (current < 0)
                current = 3;

            Grid.SetNodeType(pos.X, pos.Y, (NodeType)current);
            this.Invalidate();

            base.OnMouseClick(e);
        }

        public void Solve()
        {
            Grid.Reset();
            Node[] path = AStar.Run(Grid);
            if(path.Length == 0)
            {
                MessageBox.Show("Could not find a valid path !");
                return;
            }

            new Task(async () =>
            {
                int index = 0;
                while(index < path.Length)
                {
                    path[index].Type = NodeType.SelectedPath;
                    Invoke(new Action(()=>Invalidate()));
                    await Task.Delay(150);
                    index++;
                }
            }).Start();
        }

        public void Reset()
        {
            Grid.Reset();
            this.Invalidate();
        }
    }
}
