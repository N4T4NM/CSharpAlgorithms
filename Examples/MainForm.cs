using System;
using System.Windows.Forms;

namespace Examples
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void SolveButton_Click(object sender, EventArgs e)
            => AStarExample.Solve();

        private void ResetButton_Click(object sender, EventArgs e)
            => AStarExample.Reset();
    }
}
