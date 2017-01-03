using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using MediaKeyTimer.Models;
using Prism.Mvvm;
using Reactive.Bindings;

namespace MediaKeyTimer.ViewModels
{
    public class MainWindowViewModel : BindableBase, IDisposable
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public ReactiveProperty<int> Period { get; }

        public ReadOnlyReactiveProperty<string> ElamsedTime { get; }

        public ReactiveProperty<string> ButtonText { get; }

        public ReactiveCommand StartStopCommand { get; }


        public MainWindowViewModel()
        {
            var timer = new KeybdEventTimer(KeyInterop.VirtualKeyFromKey(Key.MediaPlayPause));
            Period = timer.Period;
            ElamsedTime = timer.ElamsedTime;

            ButtonText = timer.IsRunning.Select(x => x ? "Stop" : "Start").ToReactiveProperty();

            StartStopCommand = new ReactiveCommand();
            StartStopCommand.Subscribe(_ =>
            {
                if (timer.IsRunning.Value)
                {
                    timer.Stop();
                }
                else
                {
                    timer.Start();
                }
            });
        }

        public void Dispose()
        {
            _disposable.Dispose();
            _disposable.Clear();
        }
    }
}
