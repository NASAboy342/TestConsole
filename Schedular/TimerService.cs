using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole.Schedular
{
    public class TimerService : IHostedService, IAsyncDisposable
    {
        private readonly Task _completedTask = Task.CompletedTask;
        private int _executionCount = 0;
        private Timer? _timer;


        public Task StartAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("{Service} is running.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return _completedTask;
        }

        private void DoWork(object? state)
        {
            int count = Interlocked.Increment(ref _executionCount);
            Console.WriteLine("{Service} is working, execution count: {Count:#,0}" + count);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("{Service} is stopping.");
            // Clean up resources here if needed.
            return _completedTask;
        }

        public async ValueTask DisposeAsync()
        {
            // Dispose any resources here.
            await Task.Delay(100); // Simulate async cleanup.
        }
    }
}
