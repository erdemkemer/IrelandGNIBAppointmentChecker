using System;
using System.Threading;
using System.Threading.Tasks;

namespace NotifyIRPAppointment
{
    public class TimerProcess
    {
        private const long TimerInterval = 5000;
 
        private static object _locker = new object();
        private static Timer _timer;

        public Func<Task> Action { get; }

        public TimerProcess(Func<Task> action)
        {
            Action = action;
        }

 
        public void Start()
        {
            _timer = new Timer(Callback, Action, 0, TimerInterval);
        }
 
        public void Stop()
        {
            _timer.Dispose();
        }
 
        public void Callback(object state)
        {
            var hasLock = false;
 
            try
            {
                Monitor.TryEnter(_locker, ref hasLock);
                if (!hasLock)
                {
                    return;
                }
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
 
                var action = (Func<Task>)state;
                action().Wait();
            }
            finally
            {
                if (hasLock)
                {
                    Monitor.Exit(_locker);
                    _timer.Change(TimerInterval, TimerInterval);
                }
            }
        }
    }
}