using System;
using System.ComponentModel;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using Dota2Helper.ViewModels;

namespace Dota2Helper.Views;

public partial class TimersView : UserControl
{
    public TimersView()
    {
        InitializeComponent();
    }

    void OnItemOnPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        try
        {
            var item = (DotaTimerViewModel) sender!;
            var dataGrid = this.GetControl<DataGrid>("TimersGrid");

            foreach (var descendant in dataGrid.GetVisualDescendants())
            {
                if (descendant is DataGridRow row && row.DataContext is DotaTimerViewModel dataContext)
                {
                    Debug.WriteLine($"Row {dataContext.Name} IsVisible changed to {item.IsVisible}");
                    row.IsVisible = item.IsVisible;
                }
            }
        }
        finally
        {

        }
    }

    void TimersGrid_OnInitialized(object? sender, EventArgs e)
    {
        if (DataContext is TimersViewModel viewModel)
        {
            foreach (var item in viewModel.Timers)
            {
                item.PropertyChanged -= OnItemOnPropertyChanged;
                item.PropertyChanged += OnItemOnPropertyChanged;
            }
        }
    }
}