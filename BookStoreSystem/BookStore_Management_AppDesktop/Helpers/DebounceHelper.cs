using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Helpers
{
    public class DebounceHelper
    {
        private CancellationTokenSource? _cts;

        public async Task RunAsync(int millisecondsDelay, Func<CancellationToken, Task> action)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                await Task.Delay(millisecondsDelay, token);
                await action(token);
            }
            catch (TaskCanceledException) { }
            
        }
    }
}
