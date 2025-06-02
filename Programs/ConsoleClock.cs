namespace TestConsole.Programs
{
    public class ConsoleClock
    {
        public void Run()
        {
            while (true)
            {
                Console.Clear();
                ActionOnEachCicle();
                Thread.Sleep(1000);
            }
        }

        private void ActionOnEachCicle()
        {
            Console.CursorVisible = false;
            var clockPosition = new Position { X = Console.WindowWidth/4, Y = Console.WindowHeight/2 };
            var clockRadius = Console.WindowHeight / 2 - 3;
            DrawClockFrame(clockPosition, clockRadius);
            DrawTime(clockPosition, clockRadius , DateTime.Now);
        }

        private void DrawTime(Position clockPosition, int clockRadius, DateTime now)
        {
            DrawHour(clockPosition, clockRadius, now);
            DrawMinute(clockPosition, clockRadius, now);
            DrawSecond(clockPosition, clockRadius, now);
        }

        private void DrawHour(Position clockPosition, int clockRadius, DateTime now)
        {
            var secondsPointerLength = 50 * clockRadius / 100;
            var hour = Convert.ToInt32(now.ToString("hh"));
            var angleFrom12Oclock = 360 * hour / 12;
            var ab = GetABSideOutOfABCTriangle(angleFrom12Oclock, secondsPointerLength);
            var ac = GetACSideOutOfABCTriangle(angleFrom12Oclock, secondsPointerLength);
            var y = (int)Math.Round((ab * -1) + clockPosition.Y);
            var x = (int)Math.Round(ac + clockPosition.X);
            DrawLine(clockPosition, new Position { X = x, Y = y }, 'O');
        }

        private void DrawMinute(Position clockPosition, int clockRadius, DateTime now)
        {
            var secondsPointerLength = 75 * clockRadius / 100;
            var minute = Convert.ToInt32(now.ToString("mm"));
            var angleFrom12Oclock = 360 * minute / 60;
            var ab = GetABSideOutOfABCTriangle(angleFrom12Oclock, secondsPointerLength);
            var ac = GetACSideOutOfABCTriangle(angleFrom12Oclock, secondsPointerLength);
            var y = (int)Math.Round((ab * -1) + clockPosition.Y);
            var x = (int)Math.Round(ac + clockPosition.X);
            DrawLine(clockPosition, new Position { X = x, Y = y }, 'O');
        }

        private void DrawSecond(Position clockPosition, int clockRadius, DateTime now)
        {
            var secondsPointerLength = 90 * clockRadius / 100;
            var seconds = Convert.ToInt32(now.ToString("ss"));
            var angleFrom12Oclock = 360 * seconds / 60;
            var ab = GetABSideOutOfABCTriangle(angleFrom12Oclock, secondsPointerLength);
            var ac = GetACSideOutOfABCTriangle(angleFrom12Oclock, secondsPointerLength);
            var y = (int)Math.Round((ab * -1) + clockPosition.Y);
            var x = (int)Math.Round(ac + clockPosition.X);
            DrawLine(clockPosition, new Position { X = x, Y = y }, 'O');
        }

        private void DrawClockFrame(Position clockPosition, int clockRadius)
        {
            Draw(clockPosition, 'O');
            DrawCircle(clockPosition, clockRadius, 'O');
        }

        /// <summary>
        /// Draws a circle of given character on a position on the console by a given radius.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="symbolToDraw"></param>
        public void DrawCircle(Position position, int radius, char symbolToDraw)
        {
            for (int angle = 0; angle <= 360; angle++)
            {
                var ab = GetABSideOutOfABCTriangle(angle, radius);
                var ac = GetACSideOutOfABCTriangle(angle, radius);

                var x = (int)Math.Round(ab + position.X);
                var y = (int)Math.Round((ac * -1) + position.Y);
                Draw(x, y, symbolToDraw);
            }
        }

        /// <summary>
        /// Return the AC side length of the ABC triangle where BC is the hypotenuse.
        /// </summary>
        /// <param name="angleB"></param>
        /// <param name="bCSide"></param>
        /// <returns></returns>
        private double GetACSideOutOfABCTriangle(int angleB, int bCSide)
        {
            var radian = angleB * (Math.PI / 180);
            var ac = bCSide * Math.Sin(radian);
            return ac;
        }

        /// <summary>
        /// Return the AB side length of the ABC triangle where BC is the hypotenuse.
        /// </summary>
        /// <param name="angleB"></param>
        /// <param name="bCSide"></param>
        /// <returns></returns>
        private double GetABSideOutOfABCTriangle(int angleB, int bCSide)
        {
            var radian = angleB * (Math.PI / 180);
            var ab = bCSide * Math.Cos(radian);
            return ab;
        }

        /// <summary>
        /// Draws a line of given character between point A to point B on the console.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="symbolToDraw"></param>
        public void DrawLine(Position start, Position end, char symbolToDraw)
        {
            var isEndXSmallerThanStartX = end.X < start.X;
            var isEndYSmallerThanStartY = end.Y < start.Y;

            var distentPercentage = 0.0;
            var xUnitDistant = (double)GetXUnitDistant(start.X, end.X);
            var yUnitDistant = (double)GetYUnitDistant(start.Y, end.Y);
            var currentXUnitDistant = 0.0;
            var currentYUnitDistant = 0.0;

            if(xUnitDistant > yUnitDistant)
            {
                var x = start.X;
                do
                {
                    distentPercentage = Math.Round(currentXUnitDistant / xUnitDistant * 100);
                    currentYUnitDistant = yUnitDistant * distentPercentage / 100;
                    var y = (int)Math.Round(isEndYSmallerThanStartY ? start.Y - currentYUnitDistant : start.Y + currentYUnitDistant);

                    Draw(x, y, symbolToDraw);

                    currentXUnitDistant++;
                    currentYUnitDistant++;
                    x = x == end.X ? x : isEndXSmallerThanStartX ? x - 1 : x + 1;
                } while (x != end.X);
            }
            else
            {
                var y = start.Y;
                do
                {
                    distentPercentage = Math.Round(currentYUnitDistant / yUnitDistant * 100);
                    currentXUnitDistant = xUnitDistant * distentPercentage / 100;
                    var x = (int)Math.Round(isEndXSmallerThanStartX ? start.X - currentXUnitDistant : start.X + currentXUnitDistant);
                    
                    Draw(x, y, symbolToDraw);

                    currentXUnitDistant++;
                    currentYUnitDistant++;
                    y = y == end.Y ? y : isEndYSmallerThanStartY ? y - 1 : y + 1;
                } while (y != end.Y);
            }
            
        }

        /// <summary>
        /// Draw a single point of what character/symbol that was given on the console.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="symbolToDraw"></param>
        public void Draw(int x, int y, char symbolToDraw)
        {
            try
            {
                var actualX = x * 2;
                var actualY = y;

                Console.SetCursorPosition(actualX, actualY);
                Console.Write($"{symbolToDraw}");
            }
            catch
            {
            }
        }

        /// <summary>
        /// Draw a single point of what character/symbol that was given on the console.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="symbolToDraw"></param>
        private void Draw(Position position, char symbolToDraw)
        {
            try
            {
                var actualX = position.X * 2;
                var actualY = position.Y;

                Console.SetCursorPosition(actualX, actualY);
                Console.Write($"{symbolToDraw}");
            }
            catch
            {
            }
        }

        private int GetYUnitDistant(int y1, int y2)
        {
            return Math.Abs(y1 - y2);
        }

        private int GetXUnitDistant(int x1, int x2)
        {
            return Math.Abs(x1 - x2);
        }
    }

    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}