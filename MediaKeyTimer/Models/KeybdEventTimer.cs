using System;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using Reactive.Bindings;

namespace MediaKeyTimer.Models
{
    public class KeybdEventTimer : IDisposable
    {
        [DllImport("user32.dll")]
        public static extern uint keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private readonly ReactiveProperty<TimeSpan> _time = new ReactiveProperty<TimeSpan>(TimeSpan.Zero);
        private readonly ReactiveProperty<bool> _isRunning = new ReactiveProperty<bool>(false);
        private IDisposable _timerSubscription = null;

        public ReactiveProperty<int> Period { get; }

        public ReadOnlyReactiveProperty<string> ElamsedTime { get; }

        public ReadOnlyReactiveProperty<bool> IsRunning { get; }

        public KeybdEventTimer(int virtualKey)
        {
            Period = new ReactiveProperty<int>(60);

            IsRunning = _isRunning.ToReadOnlyReactiveProperty();

            ElamsedTime = _time
                .Select(t => t.ToString(@"h\:mm\:ss"))
                .ToReadOnlyReactiveProperty();

            _time.Where(t => t.TotalMinutes >= Period.Value)
                .Subscribe(_ =>
                {
                    Stop();
                    keybd_event((byte)virtualKey, 0, 0, UIntPtr.Zero);
                    keybd_event((byte)virtualKey, 0, 2, UIntPtr.Zero);
                });
        }

        public void Start()
        {
            if (IsRunning.Value)
                return;

            _isRunning.Value = true;
            var startTime = DateTime.Now;

            _timerSubscription =
                Observable.Interval(TimeSpan.FromMilliseconds(100))
                .Subscribe(x => _time.Value = (DateTime.Now - startTime));
        }

        public void Stop()
        {
            _timerSubscription?.Dispose();
            _timerSubscription = null;

            _isRunning.Value = false;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
