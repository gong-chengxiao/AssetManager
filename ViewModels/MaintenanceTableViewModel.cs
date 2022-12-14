﻿using System.Collections.ObjectModel;

using AssetManager.Contracts.ViewModels;
using AssetManager.Core.Contracts.Services;
using AssetManager.Core.Models;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AssetManager.ViewModels;

public class MaintenanceTableViewModel : ObservableRecipient, INavigationAware
{
    private readonly ISampleDataService _sampleDataService;

    public ObservableCollection<SampleOrder> Source { get; } = new ObservableCollection<SampleOrder>();

    public MaintenanceTableViewModel(ISampleDataService sampleDataService)
    {
        _sampleDataService = sampleDataService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();

        // TODO: Replace with real data.
        var data = await _sampleDataService.GetGridDataAsync();

        foreach (var item in data)
        {
            Source.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }
}
