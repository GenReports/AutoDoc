using AutoDoc.Core.Models;
using AutoDoc.Core.Services;
using AutoDoc.UI.Utils;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutoDoc.UI.ViewModels
{
    public partial class RepositoriesViewModel : ViewModelBase
    {
        const string StorageKey = "repositories";
        private readonly Window _currentWindow;

        public RepositoriesViewModel()
        {
            Path = string.Empty;
            Repositories = [];

            _currentWindow = ApplicationUtils.GetCurrentWindow();
            _ = LoadAsync();
        }

        public ObservableCollection<Repository> Repositories { get; }

        [ObservableProperty]
        private string path;

        [ObservableProperty]
        private Repository? selectedRepository;

        [RelayCommand]
        private async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            var repos = await JsonStorageService.LoadAsync<Repository>(StorageKey, cancellationToken) ?? [];
            Repositories.Clear();

            foreach (var repo in repos)
            {
                Repositories.Add(repo);
            }
        }

        [RelayCommand]
        private async Task AddAsync(CancellationToken cancellationToken = default)
        {
            if (!IsValidAdd(Path))
                return;

            var repo = new Repository { Path = Path };
            Repositories.Add(repo);
            await JsonStorageService.SaveAsync(StorageKey, Repositories.ToList(), cancellationToken);

            Path = string.Empty;
            await LoadAsync(cancellationToken);
        }

        private bool IsValidAdd(string path)
        {
            if (string.IsNullOrWhiteSpace(Path))
                return false;

            return !Repositories.Any(c => c.Path.Equals(path, System.StringComparison.OrdinalIgnoreCase));
        }

        [RelayCommand]
        private async Task RemoveAsync(CancellationToken cancellationToken)
        {
            if (SelectedRepository is null)
                return;

            Repositories.Remove(SelectedRepository);
            await JsonStorageService.SaveAsync(StorageKey, Repositories.ToList(), cancellationToken);
            
            Path = string.Empty;
            await LoadAsync(cancellationToken);
        }

        [RelayCommand]
        private async Task PickFolderAsync()
        {
            Path = await ApplicationUtils.PickFolderAsync(_currentWindow);
        }
    }
}