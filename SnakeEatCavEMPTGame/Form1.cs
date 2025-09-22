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
        private int directionX = 1;
        private int directionY = 0;
        private int cellSize = 50;
        private Random random = new Random();
        private Image foodImage;
        private bool isPaused = false;


        public Form1()
        {
            InitializeComponent();
            this.Text = "Змейка кушает кавчик!";
            this.DoubleBuffered = true; // чтобы не моргало
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            // картинка
            foodImage = Image.FromFile("empto.png");

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
            MessageBox.Show("Игра окончена! Результат: " + (snake.Count - 3));
            Application.Restart();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Рисуем тело змейки
            for (int i = 1; i < snake.Count; i++)
            {
                Point p = snake[i];
                e.Graphics.FillRectangle(Brushes.Green, p.X * cellSize, p.Y * cellSize, cellSize, cellSize);
                e.Graphics.DrawRectangle(Pens.DarkGreen, p.X * cellSize, p.Y * cellSize, cellSize, cellSize);
            }

            // Рисуем голову змейки с глазами
            if (snake.Count > 0)
            {
                Point head = snake[0];

                // Голова
                e.Graphics.FillRectangle(Brushes.Crimson, head.X * cellSize, head.Y * cellSize, cellSize, cellSize);
                e.Graphics.DrawRectangle(Pens.Black, head.X * cellSize, head.Y * cellSize, cellSize, cellSize);

                // Глаза (в зависимости от направления)
                int eyeSize = cellSize / 4;
                if (directionX == 1) // Движение вправо
                {
                    e.Graphics.FillEllipse(Brushes.White,
                        head.X * cellSize + cellSize - eyeSize - 2,
                        head.Y * cellSize + 2,
                        eyeSize, eyeSize);
                    e.Graphics.FillEllipse(Brushes.White,
                        head.X * cellSize + cellSize - eyeSize - 2,
                        head.Y * cellSize + cellSize - eyeSize - 2,
                        eyeSize, eyeSize);
                }
                else if (directionX == -1) // Движение влево
                {
                    e.Graphics.FillEllipse(Brushes.White,
                        head.X * cellSize + 2,
                        head.Y * cellSize + 2,
                        eyeSize, eyeSize);
                    e.Graphics.FillEllipse(Brushes.White,
                        head.X * cellSize + 2,
                        head.Y * cellSize + cellSize - eyeSize - 2,
                        eyeSize, eyeSize);
                }
                else if (directionY == -1) // Движение вверх
                {
                    e.Graphics.FillEllipse(Brushes.White,
                        head.X * cellSize + 2,
                        head.Y * cellSize + 2,
                        eyeSize, eyeSize);
                    e.Graphics.FillEllipse(Brushes.White,
                        head.X * cellSize + cellSize - eyeSize - 2,
                        head.Y * cellSize + 2,
                        eyeSize, eyeSize);
                }
                else if (directionY == 1) // Движение вниз
                {
                    e.Graphics.FillEllipse(Brushes.White,
                        head.X * cellSize + 2,
                        head.Y * cellSize + cellSize - eyeSize - 2,
                        eyeSize, eyeSize);
                    e.Graphics.FillEllipse(Brushes.White,
                        head.X * cellSize + cellSize - eyeSize - 2,
                        head.Y * cellSize + cellSize - eyeSize - 2,
                        eyeSize, eyeSize);
                }
            }

            // Рисуем еду
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

        private void TogglePause()
        {
            isPaused = !isPaused;
            if (isPaused)
            {
                gameTimer.Stop();
                ShowPauseMessege();
            }
            else gameTimer.Start();

            Invalidate();
        }

        private void ShowPauseMessege()
        {
            MessageBox.Show("Игра на паузе. Нажмите ОК, а затем клавишу ESC, чтобы продолжить игру", "ПАУЗА", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (directionY == 0 && !isPaused) { directionX = 0; directionY = -1; }
                    break;
                case Keys.Down:
                    if (directionY == 0 && !isPaused) { directionX = 0; directionY = 1; }
                    break;
                case Keys.Left:
                    if (directionX == 0 && !isPaused) { directionX = -1; directionY = 0; }
                    break;
                case Keys.Right:
                    if (directionX == 0 && !isPaused) { directionX = 1; directionY = 0; }
                    break;

                case Keys.Escape:
                    TogglePause();
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
            //вызвать базовый Dispose, чтобы не сломать форму!
            base.Dispose(disposing);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}