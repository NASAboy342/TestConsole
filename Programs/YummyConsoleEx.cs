using YummyConsole;

namespace TestConsole.Programs
{
    public class YummyConsoleEx
    {
        public void Run()
        {
            var startX = 0;
            var startY = 0;
            var endX = 50;
            var endY = 50;

            while (true)
            {
                new Line(new Point(startX, startY), new Point(endX, endY))
                {
                    ZDepth = 1,
                    foregroundColor = Color.WHITE,
                    backgroundColor = Color.GREEN,
                };

                Time.RunFrameTimer().Wait();
                startY++;
                endY++;
            }
        }
    }
}