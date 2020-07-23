using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using ThreadState = System.Threading.ThreadState;

namespace SimpleGameEngine.Core
{
    public class GameLoop
    {

        private static GameLoop _instance;

        public static GameLoop Instance => _instance ?? (_instance = new GameLoop());

        public const int FramePerSec = 25;
        public const int SkipTicks = 1000 / FramePerSec;
        private const int MaxSkip = 5;
        private CancellationTokenSource _token = new CancellationTokenSource();

        private Thread _loopThread;

        private GameLoop()
        {
            _loopThread = CreateThread();
        }
        
        private long CurrentTickCount => DateTime.Now.Ticks;
        
        public bool IsRunning { get; set; }

        private Thread CreateThread()
        {
            return new Thread(Loop)
            {
                IsBackground = true
            };
        } 
        
        public void Start()
        {
            if (IsRunning)
                return;
            IsRunning = true;
            if (_loopThread.ThreadState == ThreadState.Stopped)
                _loopThread = CreateThread();
            _loopThread.Start();
        }

        public void Stop()
        {
            IsRunning = false;
        }
        
        private void Loop()
        {
            var nextTick = CurrentTickCount;
            while (IsRunning)
            {
                var loops = 0;
                while (CurrentTickCount > nextTick && loops < SkipTicks)
                {
                    UpdateState();
                    nextTick += SkipTicks;
                    loops++;
                }
                Rendering();
            }
        }

        private void UpdateState()
        {
            SyncInvoke(() => UpdateStateEvent?.Invoke());
        }

        private void Rendering()
        {
            SyncInvoke(() => RenderEvent?.Invoke());
        }
        
        private void SyncInvoke(Action action)
        {
            try
            {
                Application.Current.Dispatcher?.Invoke(action);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}\n {e.StackTrace}");
            }
            
        }

        public delegate void UpdateStateHandler();
        public static event UpdateStateHandler UpdateStateEvent;

        public delegate void RenderHandler();
        public static event RenderHandler RenderEvent;

    }
}