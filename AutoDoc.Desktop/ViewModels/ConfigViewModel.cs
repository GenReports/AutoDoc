using AutoDoc.Core.Models;
using AutoDoc.Core.Services;
using AutoDoc.UI.Utils;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutoDoc.UI.ViewModels
{
    public partial class ConfigViewModel : ViewModelBase
    {
        const string StorageKey = "configs";
        private readonly Window _currentWindow;

        public ConfigViewModel()
        {
            Name = string.Empty;
            ApiKey = string.Empty;
            Culture = string.Empty;
            OutputPath = string.Empty;
            CompletionsUri = string.Empty;
            AppSettings = [];

            _currentWindow = ApplicationUtils.GetCurrentWindow();

            _ = InitializeAsync();
            _ = LoadAsync();
        }

        public ObservableCollection<AppSettings> AppSettings { get; }

        [ObservableProperty]
        private AppSettings? selectedItem;

        [ObservableProperty]

        private Guid id;

        [ObservableProperty]

        private string name;

        [ObservableProperty]

        private string culture;

        [ObservableProperty]
        private string outputPath;

        [ObservableProperty]

        private string completionsUri;

        [ObservableProperty]

        private string apiKey;

        [ObservableProperty]

        private double modelTemperature;

        [ObservableProperty]
        private int maxRetries;

        [ObservableProperty]
        private int delayMilliseconds;

        [RelayCommand]
        private async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            var loaded = await JsonStorageService.LoadAsync<AppSettings>(StorageKey, cancellationToken) ?? [];

            AppSettings.Clear();

            foreach (var item in loaded)
            {
                AppSettings.Add(item);
            }
        }

        [RelayCommand]
        private async Task AddAsync(CancellationToken cancellationToken = default)
        {
            var newItem = new AppSettings
            {
                Name = Name,
                Culture = Culture,
                OutputPath = OutputPath,
                CompletionsUri = CompletionsUri,
                ApiKey = ApiKey,
                ModelTemperature = ModelTemperature,
                MaxRetries = MaxRetries,
                DelayMilliseconds = DelayMilliseconds
            };

            if (!IsValidAdd(newItem))
                return;

            AppSettings.Add(newItem);
            await SaveAllAsync(cancellationToken);
            ClearFields();
        }

        private bool IsValidAdd(AppSettings appSettings)
        {
            if (string.IsNullOrWhiteSpace(appSettings.Name))
                return false;

            if (!CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Any(c => c.Name.Equals(appSettings.Culture, StringComparison.OrdinalIgnoreCase)))
                return false;

            if (string.IsNullOrWhiteSpace(appSettings.CompletionsUri))
                return false;

            return !AppSettings.Any(c => c.Name.Equals(appSettings.Name, StringComparison.OrdinalIgnoreCase));
        }

        [RelayCommand]
        private async Task UpdateAsync(CancellationToken cancellationToken = default)
        {
            if (SelectedItem is null)
                return;

            var updateItem = new AppSettings
            {
                Name = Name,
                Culture = Culture,
                OutputPath = OutputPath,
                CompletionsUri = CompletionsUri,
                ApiKey = ApiKey,
                ModelTemperature = ModelTemperature,
                MaxRetries = MaxRetries,
                DelayMilliseconds = DelayMilliseconds
            };

            if (!IsValidUpdate(updateItem))
                return;

            SelectedItem.Name = updateItem.Name;
            SelectedItem.ApiKey = updateItem.ApiKey;
            SelectedItem.Culture = updateItem.Culture;
            SelectedItem.MaxRetries = updateItem.MaxRetries;
            SelectedItem.OutputPath = updateItem.OutputPath;
            SelectedItem.CompletionsUri = updateItem.CompletionsUri;
            SelectedItem.ModelTemperature = updateItem.ModelTemperature;
            SelectedItem.DelayMilliseconds = updateItem.DelayMilliseconds;

            await SaveAllAsync(cancellationToken);
            ClearFields();
        }

        private bool IsValidUpdate(AppSettings appSettings)
        {
            if (string.IsNullOrWhiteSpace(appSettings.Name))
                return false;

            if (!CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Any(c => c.Name.Equals(appSettings.Culture, StringComparison.OrdinalIgnoreCase)))
                return false;

            if (string.IsNullOrWhiteSpace(appSettings.CompletionsUri))
                return false;

            return !AppSettings.Any(c => c.Name.Equals(appSettings.Name, StringComparison.OrdinalIgnoreCase) &&
                                         c.Id != appSettings.Id);
        }

        [RelayCommand]
        private async Task DeleteAsync(CancellationToken cancellationToken = default)
        {
            if (SelectedItem is null)
                return;

            AppSettings.Remove(SelectedItem);
            await SaveAllAsync(cancellationToken);
        }

        private async Task SaveAllAsync(CancellationToken cancellationToken = default)
        {
            await JsonStorageService.SaveAsync(StorageKey, AppSettings.ToList(), cancellationToken);
            await LoadAsync(cancellationToken);
        }

        private void ClearFields()
        {
            MaxRetries = 0;
            Name = string.Empty;
            ModelTemperature = 0;
            ApiKey = string.Empty;
            DelayMilliseconds = 0;
            Culture = string.Empty;
            OutputPath = string.Empty;
            CompletionsUri = string.Empty;
        }

        [RelayCommand]
        private async Task PickFolderAsync()
        {
            OutputPath = await ApplicationUtils.PickFolderAsync(_currentWindow);
        }

        [RelayCommand]
        private void Clear()
        {
            SelectedItem = null;
            ClearFields();
        }

        [RelayCommand]
        private async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            var defaultAppSettings = GetDefaultSettings();
            AppSettings.Add(defaultAppSettings);
            await JsonStorageService.InitializeAsync(StorageKey, AppSettings.ToList(), cancellationToken);
        }

        private static AppSettings GetDefaultSettings()
        {
            return new AppSettings
            {
                MaxRetries = 3,
                Name = "Default",
                Culture = "en-US",
                OutputPath = "outputs",
                ApiKey = string.Empty,
                ModelTemperature = 0.7,
                DelayMilliseconds = 5_000,
                Id = Guid.Parse("61fad88a-df29-425e-9c69-fd73cdeea6d3"),
                CompletionsUri = "http://localhost:1234/v1/chat/completions",
            };
        }

        partial void OnSelectedItemChanged(AppSettings? value)
        {
            if (SelectedItem is null)
                return;

            Name = SelectedItem.Name;
            ApiKey = SelectedItem.ApiKey;
            Culture = SelectedItem.Culture;
            MaxRetries = SelectedItem.MaxRetries;
            OutputPath = SelectedItem.OutputPath;
            CompletionsUri = SelectedItem.CompletionsUri;
            ModelTemperature = SelectedItem.ModelTemperature;
            DelayMilliseconds = SelectedItem.DelayMilliseconds;
        }
    }
}