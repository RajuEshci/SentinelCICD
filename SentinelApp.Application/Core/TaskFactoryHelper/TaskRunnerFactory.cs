namespace SentinelApp.Application.Core
{
    public class TaskRunnerFactory : AsyncHelper ,IDisposable
    {
        private readonly List<Task> tasks = new List<Task>();

        public static TaskRunnerFactory GetTaskRunner
        {
            get
            {
                return new TaskRunnerFactory();
            }
        }

        public static Task<T> RunTask<T>(Func<T> func)
        {
            return Task.Run<T>(() => func());
        }

        public void AddTaskinQueue(Func<Task> func)
        {
            tasks.Add(func());
        }
        public void AddTaskinQueue(Task task)
        {
            tasks.Add(task);
        }
        public void FinallyRunTask()
        {
            Task.WaitAll(tasks.ToArray());
            //Task.Factory.StartNew(() => tasks.ForEach(task => task.Start()));
        }

        public void RunTaskAsync(Func<Task> func)
        {
            RunSync(()=> func());
        }
        public TResult RunTaskAsync<TResult>(Func<Task<TResult>> func)
        {
            return RunSync<TResult>(() => func());
        }

        public void Dispose()
        {
            
        }
    }
}
