using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Avalonia.Threading;

namespace D2Helper.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

public class SortableObservableCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged, IComparable<T>
{
    private bool _suppressCollectionChanged;

    public SortableObservableCollection()
    {
        CollectionChanged += OnCollectionChanged;
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (_suppressCollectionChanged) return;

        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (T item in e.NewItems)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (T item in e.OldItems)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }
    }

    private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DotaTimerViewModel.SortOrder))
        {
            QueueSort();
        }
    }

    private void QueueSort()
    {
        Dispatcher.UIThread.InvokeAsync(Sort);
    }

    public void Sort()
    {
        _suppressCollectionChanged = true;

        var sortedItems = this.OrderBy(x => x).ToList();
        for (int i = 0; i < sortedItems.Count; i++)
        {
            var oldIndex = IndexOf(sortedItems[i]);
            if (oldIndex != i)
            {
                Move(oldIndex, i);
            }
        }

        _suppressCollectionChanged = false;

        // Manually raise the CollectionChanged event to notify listeners after sorting
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}
