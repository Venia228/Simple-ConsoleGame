using System;
using System.Timers;

namespace ConsoleGame
{
    internal class Program
    {
        private static long lastTick;
        private static Random rnd = new Random();

        public static int plPositionX;
        public static int plPositionY;

        public static string[,] worldMap;
        public static int sizeX;
        public static int sizeY;

        public static ulong points = 0;
        public static ulong movements = 0;
        public static int timerTime;
        static void Main()
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine("\t\t\t\t\tХодилка-бродилка by Veniamin Shein\n");
            Console.BackgroundColor = ConsoleColor.Black;

            Console.WriteLine("\tЦель: ходить и собирать монетки обходя зелёные припятсваия\nПри сборе монеток увеличивается счёт\n");

            Console.WriteLine("Нажмите на любую улавишу, чтобы продолжить...");
            Console.ReadLine();

            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\t\t\t\t\tРазмер игрового пространства\n");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write("Размер по горизонтали (min = 10, max = 100)(Целое) = ");
            sizeX = int.Parse(Console.ReadLine());

            Console.Write("Размер по вертикали (min = 5, max = 25)(Целое) = ");
            sizeY = int.Parse(Console.ReadLine());

            Console.Write("Частота появления припятствий (min = 2, max = 20)(Целое) = ");
            int obstaclesChance = int.Parse(Console.ReadLine());

            Console.Write("Время таймера (В секундах)(Целое) = ");
            timerTime = int.Parse(Console.ReadLine());

            worldMap = GnerateWroldMap(sizeX, sizeY, obstaclesChance);

            PlaceCoin();

            lastTick = DateTime.Now.Ticks;
            Timer timer = new Timer(1000);
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = true;
            timer.Enabled = true;

            while (timerTime > 0)
            {
                while (!Console.KeyAvailable && timerTime > 0)
                {
                    Update();

                    long currentTick = DateTime.Now.Ticks;
                    while (currentTick - lastTick < TimeSpan.TicksPerSecond && !Console.KeyAvailable)
                    {
                        currentTick = DateTime.Now.Ticks;
                    }
                    lastTick = currentTick;
                }

                if (timerTime <= 0)
                {
                    continue;
                }

                Move();
            }

            timer.Enabled = false;

            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\t\t\t\tИГРА ОКОНЧЕНА!");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine($"Cчёт: {points}");
            Console.WriteLine($"Шаги: {movements}");

            Console.ReadLine();
        }

        private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            timerTime--;
        }

        private static void PlaceCoin()
        {
            int posX = rnd.Next(worldMap.Length / sizeY);
            int posY = rnd.Next(worldMap.Length / sizeX);

            if(worldMap[posY, posX] == "@")
            {
                PlaceCoin();
            }
            else
            {
                worldMap[posY, posX] = "$";
            }
        }

        private static void Move()
        {
            int oldPositionX = plPositionX;
            int oldPositionY = plPositionY;

            ConsoleKey key = Console.ReadKey().Key;

            switch (key)
            {
                case ConsoleKey.W:

                    plPositionY--;
                    movements++;
                    break;

                case ConsoleKey.S:

                    plPositionY++;
                    movements++;
                    break;

                case ConsoleKey.A:

                    plPositionX--;
                    movements++;
                    break;

                case ConsoleKey.D:

                    plPositionX++;
                    movements++;
                    break;
            }

            if (plPositionY >= worldMap.Length / sizeX || plPositionY < 0)
            {
                plPositionX = oldPositionX;
                plPositionY = oldPositionY;
                movements--;
                return;
            }
            if (plPositionX >= worldMap.Length / sizeY || plPositionX < 0)
            {
                plPositionX = oldPositionX;
                plPositionY = oldPositionY;
                movements--;
                return;
            }
            if (worldMap[plPositionY, plPositionX] == "#")
            {
                plPositionX = oldPositionX;
                plPositionY = oldPositionY;
                movements--;
                return;
            }

            if (worldMap[plPositionY, plPositionX] == "$")
            {
                points++;
                PlaceCoin();
            }

            worldMap[oldPositionY, oldPositionX] = ".";
            worldMap[plPositionY, plPositionX] = "@";
        }

        private static void Update()
        {
            Console.Clear();

            for (int y = 0; y < worldMap.Length / sizeX; y++)
            {
                for (int x = 0; x < worldMap.Length / sizeY; x++)
                {
                    if (worldMap[y, x] == "#")
                        Console.ForegroundColor = ConsoleColor.Green;
                    else if (worldMap[y, x] == "@")
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    else if (worldMap[y, x] == "$")
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    else
                        Console.ForegroundColor = ConsoleColor.White;

                    Console.Write(worldMap[y, x]);
                }

                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\nПозиция игрока: X = {plPositionX}, Y = {plPositionY}");
            Console.WriteLine($"Оставшиеся время: {timerTime}сек");
            Console.WriteLine($"Очки: {points}");
            Console.WriteLine($"Ходы: {movements}");
        }

        private static string[,] GnerateWroldMap(int sizeX, int sizeY, int obstaclesChance)
        {
            sizeX = Math.Clamp(sizeX, 10, 100);
            sizeY = Math.Clamp(sizeY, 5, 25);
            obstaclesChance = Math.Clamp(obstaclesChance, 2, 20);

            string[,] worldMap = new string[sizeY, sizeX];

            for (int i = 0; i < worldMap.Length / sizeX; i++)
            {
                for (int j = 0; j < worldMap.Length / sizeY; j++)
                {
                    if (rnd.Next(0, obstaclesChance) == 1)
                        worldMap[i, j] = "#";
                    else
                        worldMap[i, j] = ".";
                }
            }

            plPositionX = sizeX / 2;
            plPositionY = sizeY / 2;

            worldMap[plPositionY, plPositionX] = "@";

            return worldMap;
        }
    }
}
