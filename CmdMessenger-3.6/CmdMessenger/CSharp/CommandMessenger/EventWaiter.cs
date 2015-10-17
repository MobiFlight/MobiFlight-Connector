using System.Threading;

namespace CommandMessenger
{
    public class EventWaiter
    {
        public enum WaitState
        {
            Quit,
            TimeOut,
            Normal
        }

        readonly object _key = new object();
        bool _block;
        bool _quit;


        public EventWaiter()
        {
            lock (_key)
            {
                _block = true;
            }
        }

        public EventWaiter(bool block)
        {
            lock (_key)
            {
                _block = block;
            }
        }

        public WaitState Wait(int timeOut)
        {
            lock (_key)
            {
                // Check if signal has already been raised before wait
                if (_quit)
                {
                    _quit = false;
                    return WaitState.Quit;
                }
                if (!_block)
                {
                    _block = true;
                    return WaitState.Normal;
                }
                
                // Set time 
                var millisBefore = TimeUtils.Millis;
                long elapsed = 0;

                // Wait under conditions
                while (elapsed < timeOut && _block && !_quit)
                {
                    Monitor.Wait(_key,timeOut);
                    elapsed = TimeUtils.Millis - millisBefore;
                }

                _block = true;
                // Check if signal has already been raised after wait                
                if (_quit)
                {
                    _quit = false;
                    return WaitState.Quit;
                }
                return elapsed >= timeOut ? WaitState.TimeOut : WaitState.Normal;
            }
        }

        public void Set()
        {
            lock (_key)
            {
                _block = false;
                Monitor.Pulse(_key);
            }
        }

        public void Reset()
        {
            lock (_key)
            {
                _block = true;
                Monitor.Pulse(_key);
            }
        }

        public void Quit()
        {
            lock (_key)
            {
                _quit = true;
                Monitor.Pulse(_key);
            }
        }

    }
}
