using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MySnakeGame
{
    public partial class Form1 : Form
    {
        private Timer gameTimer;
        private List<Point> snake;
        private Point food;
        private int directionX = 1; // 1 = вправо
        private int directionY = 0;
        private int cellSize = 20;
        private Random random = new Random();
        private Image foodImage; // Твоя картинка!

        public Form1()
        {
            InitializeComponent();
            this.Text = "Змейка с моей картинкой!";
            this.DoubleBuffered = true; // чтобы не моргало
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            // Загрузи свою картинку
            foodImage = Image.FromFile("empto.png"); // или Properties.Resources.target если через ресурсы

            StartGame();
        }

        private void StartGame()
        {
            snake = new List<Point> { new Point(5, 5), new Point(4, 5), new Point(3, 5) };
            SpawnFood();
            gameTimer = new Timer();
            gameTimer.Interval = 150; // скорость змейки
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
            this.KeyDown += Form1_KeyDown;
        }

        private void SpawnFood()
        {
            food = new Point(
                random.Next(0, this.ClientSize.Width / cellSize),
                random.Next(0, this.ClientSize.Height / cellSize)
            );
        }

        private void GameLoop(object sender, EventArgs e)
        {
            MoveSnake();
            CheckCollision();
            this.Invalidate(); // перерисовать форму
        }

        private void MoveSnake()
        {
            Point head = snake[0];
            Point newHead = new Point(head.X + directionX, head.Y + directionY);
            snake.Insert(0, newHead);

            // Если съела еду
            if (newHead == food)
            {
                SpawnFood(); // новая еда
            }
            else
            {
                snake.RemoveAt(snake.Count - 1); // убрать хвост
            }
        }

        private void CheckCollision()
        {
            Point head = snake[0];

            // Стенки
            if (head.X < 0 || head.Y < 0 ||
                head.X >= this.ClientSize.Width / cellSize ||
                head.Y >= this.ClientSize.Height / cellSize)
            {
                GameOver();
                return;
            }

            // Себя
            for (int i = 1; i < snake.Count; i++)
            {
                if (head == snake[i])
                {
                    GameOver();
                    return;
                }
            }
        }

        private void GameOver()
        {
            gameTimer.Stop();
            MessageBox.Show("Game Over! Score: " + (snake.Count - 3));
            Application.Restart();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Рисуем змейку
            foreach (Point p in snake)
            {
                e.Graphics.FillRectangle(Brushes.Green, p.X * cellSize, p.Y * cellSize, cellSize, cellSize);
            }

            // Рисуем ЕДУ — твою картинку!
            if (foodImage != null)
            {
                e.Graphics.DrawImage(
                    foodImage,
                    food.X * cellSize,
                    food.Y * cellSize,
                    cellSize,
                    cellSize
                );
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (directionY == 0) { directionX = 0; directionY = -1; }
                    break;
                case Keys.Down:
                    if (directionY == 0) { directionX = 0; directionY = 1; }
                    break;
                case Keys.Left:
                    if (directionX == 0) { directionX = -1; directionY = 0; }
                    break;
                case Keys.Right:
                    if (directionX == 0) { directionX = 1; directionY = 0; }
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                gameTimer?.Dispose();
                foodImage?.Dispose();
            }
            // ВАЖНО: вызвать базовый Dispose, чтобы не сломать форму!
            base.Dispose(disposing);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}