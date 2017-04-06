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
                .Select(t => TimeSpan.FromTicks(Math.Max(t.Ticks, 0)).ToString(@"h\:mm\:ss"))
                .ToReadOnlyReactiveProperty();

            _time.Where(t => t.TotalMinutes <= 0)
                .Subscribe(_ =>
                {
                    if (IsRunning.Value)
                    {
                        keybd_event((byte)virtualKey, 0, 0, UIntPtr.Zero);
                        keybd_event((byte)virtualKey, 0, 2, UIntPtr.Zero);
                    }
                    Stop();
                });
        }

        public void Start()
        {
            if (IsRunning.Value)
                return;

            _isRunning.Value = true;
            var countdown = DateTime.Now.AddMinutes(Period.Value);

            _timerSubscription =
                Observable.Interval(TimeSpan.FromMilliseconds(100))
                .Subscribe(x => _time.Value = (countdown - DateTime.Now));
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
