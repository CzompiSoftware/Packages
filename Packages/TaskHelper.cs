namespace Packages;
internal class TaskHelper
{
    internal static void RecurringTask(Action action, int seconds, CancellationToken token)
    {
        if (action == null)
            return;
        Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                action();
                await Task.Delay(TimeSpan.FromSeconds(seconds), token);
            }
        }, token);
    }
}
