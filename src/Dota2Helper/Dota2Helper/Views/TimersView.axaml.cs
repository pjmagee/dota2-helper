using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
            var item = (DotaTimerViewModel)sender!;
            var dataGrid = this.GetControl<DataGrid>("TimersGrid");

            foreach (DataGridRow row in dataGrid.GetVisualDescendants().OfType<DataGridRow>())
            {
                if (row.DataContext is DotaTimerViewModel dataContext)
                {
                    if (dataContext == item)
                    {
                        row.IsVisible = item.IsVisible;
                        row.Classes.Remove("visible");
                        row.Classes.Remove("hidden");

                        if (item.IsVisible)
                        {
                            row.Classes.Add("visible");
                        }
                        else
                        {
                            row.Classes.Add("hidden");
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }

    void TimersGrid_OnInitialized(object? sender, EventArgs e)
    {
        // if (DataContext is TimersViewModel viewModel)
        // {
        //     viewModel.Timers.CollectionChanged += (o, args) => { IsVisibleCalculation(args); };
        //
        //     foreach (var item in viewModel.Timers)
        //     {
        //         item.PropertyChanged -= OnItemOnPropertyChanged;
        //         item.PropertyChanged += OnItemOnPropertyChanged;
        //     }
        // }
    }

    void IsVisibleCalculation(NotifyCollectionChangedEventArgs args)
    {
        // foreach (var item in args.NewItems!.OfType<DotaTimerViewModel>())
        // {
        //     item.PropertyChanged -= OnItemOnPropertyChanged;
        //     item.PropertyChanged += OnItemOnPropertyChanged;
        // }
    }
}