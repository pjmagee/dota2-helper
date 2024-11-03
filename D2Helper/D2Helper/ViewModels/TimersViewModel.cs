using System;
using System.Collections.ObjectModel;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using D2Helper.Models;
using D2Helper.Services;
using D2Helper.Views;

namespace D2Helper.ViewModels;

public class TimersViewModel : ViewModelBase
{
    readonly TimerService _timerService;
    readonly GameStateService _gameStateService;
    TimeSpan _time;

    public TimersViewModel(TimerService timerService, GameStateService gameStateService)
    {
        _timerService = timerService;
        _gameStateService = gameStateService;
        _gameStateService.TimeElapsed += (sender, time) =>
        {
            foreach (var timer in Timers)
            {
                timer.Update(time);
            }

            Time = time;
        };

        OpenSettingsCommand = new RelayCommand(() =>
        {
            var settings = new SettingsWindow()
            {
                DataContext = new SettingsViewModel(_timerService)
            };

            settings.Show();
        });

        MoveWindowCommand = new RelayCommand<HoldingState>(state =>
        {
            if (state == HoldingState.Started)
            {

            }
        });
    }

    public TimersViewModel() : this(new TimerService(), new FakeGameStateService())
    {

    }

    public ObservableCollection<DotaTimer> Timers => _timerService.Timers;
    public IRelayCommand OpenSettingsCommand { get; }

    public TimeSpan Time
    {
        get => _time;
        set => SetProperty(ref _time, value);
    }

    public IRelayCommand<HoldingState> MoveWindowCommand { get; }
}