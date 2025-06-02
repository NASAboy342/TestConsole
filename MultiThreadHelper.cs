using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    public class MultiThreadHelper
    {
        public static async Task<List<Tresponse>> Execute<Tresponse>(int threads, Func<List<Tresponse>> function)
        {
            var tasks = new List<Task>();
            var result = new List<Tresponse>();
            for (int i = 0; i < threads; i++)
            {
                tasks.Add(Task.Run(() => result.AddRange(function())));
            }
            await Task.WhenAll(tasks);
            return result;
        }
        public static async Task<List<Tresponse>> Execute<Tresponse>(int threads, Func<Task<List<Tresponse>>> function)
        {
            var tasks = new List<Task>();
            var result = new List<Tresponse>();
            for (int i = 0; i < threads; i++)
            {
                tasks.Add(Task.Run(async () => result.AddRange(await function())));
            }
            await Task.WhenAll(tasks);
            return result;
        }
    }
}
