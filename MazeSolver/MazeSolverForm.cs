/*
 * Form for showing the maze to the user.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MazeSolver
{
    public partial class MazeSolverForm : Form
    {
        private GenAlg m_gen;
        public MazeSolverForm()
        {
            this.InitializeComponent();
            this.Controls.Add(this.m_MazeSurface);
            this.Enter += (StartRun);
            this.KeyUp += (KeyPressed);
        }

        public void CreateBitmapAtRuntime(MazeMap map)
        {
            var maze_size = this.m_MazeSurface.Size;
            Bitmap flag = new Bitmap(maze_size.Width, maze_size.Height);
            Graphics flagGraphics = Graphics.FromImage(flag);
            flagGraphics.Clear(Color.White);
            Grid grid = new Grid(map, maze_size, flagGraphics);
            grid.DrawGrid();
            m_MazeSurface.Image = flag;
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Enter:
                {
                    OnEnter(e);
                    break;
                }
                case Keys.Space:
                {
                    StopSearch();
                    break;
                }
            }
        }

        private void StopSearch()
        {
            m_gen.Searching = false;
        }

        private async void StartRun(object sender, EventArgs e)
        {
            Console.WriteLine("Welcome to the jungle!");
            if(m_gen != null && m_gen.Searching)
            {
                m_gen.Searching = false;
                return;
            }

            m_gen = new GenAlg(GenAlg.POPULATION_SIZE);
            m_gen.Searching = true;
            Func <bool> find_solution = () =>
            {
                while(m_gen.Searching)
                {
                    m_gen.Epoch();
                    Console.WriteLine("Generation: {0}, Fittest genome score: {1}, Fittest genome: {2}", m_gen.Generation, m_gen.FittestGenome.Fitness, m_gen.FittestGenome);
                    CreateBitmapAtRuntime(m_gen.BestRun);
                }
                return true;
            };

            Task<bool> solution = Task.Run(find_solution);
            await solution;
            Console.WriteLine("Found solutoin: {0}", m_gen.FittestGenome);
        }
    }

    public class Grid
    {
        private int m_cell_border;
        private readonly MazeMap m_maze;
        private readonly Graphics m_graphics;
        private int m_cell_width;
        private int m_cell_height;

        public Grid(MazeMap maze, Size surface_size, Graphics surface)
        {
            m_cell_border = 2;
            m_maze = maze;
            m_graphics = surface;
            m_cell_width = surface_size.Width / MAP_SIZE.WIDTH;
            m_cell_height = surface_size.Height / MAP_SIZE.HEIGHT;
        }

        public void DrawGrid()
        {
            for(int x = 0; x < MAP_SIZE.WIDTH; ++x)
            {
                for(int y = 0; y < MAP_SIZE.HEIGHT; ++y)
                {
                    var step_value = m_maze.Memory[y, x];
                    if(step_value > 0)
                    {
                        DrawCell(x, y, Brushes.Orange);
                        continue;
                    }
                    var cell_value = m_maze.Map[y, x];
                    if(cell_value == 5)
                    {
                        DrawCell(x, y, Brushes.Red);
                    }
                    else if(cell_value == 8)
                    {
                        DrawCell(x, y, Brushes.Green);
                    }
                    else if(cell_value == 1)
                    {
                        DrawCell(x, y, Brushes.Black);
                    }
                }
            }
        }

        private void DrawCell(int x, int y, Brush color)
        {
            if(x >= MAP_SIZE.WIDTH || y >= MAP_SIZE.HEIGHT)
            {
                throw new ArgumentOutOfRangeException("x, or y");
            }

            int left = x * m_cell_width + m_cell_border;
            int right = left + m_cell_width;
            int top = y * m_cell_height + m_cell_border;
            int bottom = top + m_cell_height;
            Rectangle rect = new Rectangle(left, top, m_cell_width - m_cell_border, m_cell_height - m_cell_border);
            m_graphics.FillRectangle(color, rect);
        }
    }
}
