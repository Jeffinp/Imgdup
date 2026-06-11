using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Imgdup.App.Services;
using Imgdup.Core.Models;
using Imgdup.Core.Scanning;

namespace Imgdup.App.ViewModels;

public sealed partial class MainViewModel : ObservableObject, IDisposable
{
    private const int ThumbnailDecodeSize = (int)ThumbnailSize.Large; // decode once at max; views just scale down

    private readonly ImageScanEngine _engine;
    private readonly IFolderPickerService _folderPicker;
    private readonly IRecycleBinService _recycleBin;
    private readonly IThumbnailService _thumbnails;
    private readonly IUserDialogService _dialogs;

    private readonly List<DuplicateGroupViewModel> _groups = [];
    private CancellationTokenSource? _scanCts;

    public MainViewModel(
        ImageScanEngine engine,
        IFolderPickerService folderPicker,
        IRecycleBinService recycleBin,
        IThumbnailService thumbnails,
        IUserDialogService dialogs)
    {
        _engine = engine;
        _folderPicker = folderPicker;
        _recycleBin = recycleBin;
        _thumbnails = thumbnails;
        _dialogs = dialogs;

        ItemsView = CollectionViewSource.GetDefaultView(Items);
        ItemsView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ImageItemViewModel.Group)));
        ApplySort();
    }

    public ObservableCollection<string> Folders { get; } = [];

    /// <summary>Flat collection of all duplicate items; grouped and sorted through <see cref="ItemsView"/>.</summary>
    public ObservableCollection<ImageItemViewModel> Items { get; } = [];

    /// <summary>Grouped + sorted view bound by the gallery. Grouping keeps a virtualized panel cohesive per cluster.</summary>
    public ICollectionView ItemsView { get; }

    public static int ThumbnailDecodePixels => ThumbnailDecodeSize;

    public IReadOnlyList<SortMode> SortModes { get; } = Enum.GetValues<SortMode>();
    public IReadOnlyList<ThumbnailSize> ThumbnailSizes { get; } = Enum.GetValues<ThumbnailSize>();

    [ObservableProperty] private bool _recursive = true;
    [ObservableProperty] private bool _detectNearDuplicates = true;
    [ObservableProperty] private int _nearThreshold = 6;

    [ObservableProperty] private ThumbnailSize _selectedThumbnailSize = ThumbnailSize.Medium;
    [ObservableProperty] private SortMode _selectedSort = SortMode.SizeDescending;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ScanCommand))]
    [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
    private bool _isScanning;

    [ObservableProperty] private double _progressFraction;
    [ObservableProperty] private string _statusText = "Selecione pastas e inicie a verificação.";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteSelectedCommand))]
    private int _selectedCount;

    /// <summary>Display size for thumbnails (bound to image Width/Height in the view).</summary>
    public int ThumbnailPixels => (int)SelectedThumbnailSize;

    partial void OnSelectedThumbnailSizeChanged(ThumbnailSize value) => OnPropertyChanged(nameof(ThumbnailPixels));

    partial void OnSelectedSortChanged(SortMode value) => ApplySort();

    [RelayCommand]
    private void PickFolders()
    {
        foreach (var folder in _folderPicker.PickFolders())
        {
            if (!Folders.Contains(folder, StringComparer.OrdinalIgnoreCase))
                Folders.Add(folder);
        }
        ScanCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void RemoveFolder(string? folder)
    {
        if (folder is not null)
            Folders.Remove(folder);
        ScanCommand.NotifyCanExecuteChanged();
    }

    private bool CanScan() => !IsScanning && Folders.Count > 0;

    [RelayCommand(CanExecute = nameof(CanScan))]
    private async Task ScanAsync()
    {
        ClearGroups();
        IsScanning = true;
        ProgressFraction = 0;
        _scanCts = new CancellationTokenSource();

        var options = new ScanOptions
        {
            Folders = Folders.ToList(),
            Recursive = Recursive,
            DetectNearDuplicates = DetectNearDuplicates,
            NearDuplicateThreshold = NearThreshold,
        };

        var progress = new Progress<ScanProgress>(p =>
        {
            ProgressFraction = p.Fraction;
            StatusText = p.Phase switch
            {
                ScanPhase.Enumerating => "Procurando arquivos…",
                ScanPhase.Hashing => $"Analisando {p.Processed}/{p.Total}…",
                ScanPhase.Clustering => "Agrupando duplicatas…",
                ScanPhase.Completed => "Concluído.",
                _ => StatusText,
            };
        });

        try
        {
            var groups = await _engine.ScanAsync(options, progress, _scanCts.Token).ConfigureAwait(true);
            foreach (var group in groups)
                AddGroup(new DuplicateGroupViewModel(group, _thumbnails));

            StatusText = _groups.Count == 0
                ? "Nenhuma duplicata encontrada."
                : $"{_groups.Count} grupos de duplicatas encontrados.";
        }
        catch (OperationCanceledException)
        {
            StatusText = "Verificação cancelada.";
        }
        finally
        {
            IsScanning = false;
            _scanCts?.Dispose();
            _scanCts = null;
        }
    }

    private bool CanCancel() => IsScanning;

    [RelayCommand(CanExecute = nameof(CanCancel))]
    private void Cancel() => _scanCts?.Cancel();

    [RelayCommand]
    private void SelectExtras()
    {
        foreach (var item in Items)
            item.IsSelected = !item.IsKeepCandidate;
    }

    [RelayCommand]
    private void ClearSelection()
    {
        foreach (var item in Items)
            item.IsSelected = false;
    }

    private bool CanDelete() => SelectedCount > 0;

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task DeleteSelectedAsync()
    {
        var selected = Items.Where(i => i.IsSelected).ToList();
        if (selected.Count == 0)
            return;

        if (!_dialogs.Confirm(
                $"Enviar {selected.Count} arquivo(s) para a Lixeira? Você poderá restaurá-los pela Lixeira do Windows.",
                "Confirmar exclusão"))
        {
            return;
        }

        var paths = selected.Select(i => i.Path).ToList();
        var results = await Task.Run(() => _recycleBin.SendToRecycleBin(paths)).ConfigureAwait(true);

        var failedPaths = results.Where(r => !r.Success).Select(r => r.Path).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var item in selected.Where(i => !failedPaths.Contains(i.Path)))
            RemoveItem(item);

        PruneEmptyGroups();

        int deleted = results.Count(r => r.Success);
        StatusText = $"{deleted} enviado(s) para a Lixeira.";
        if (failedPaths.Count > 0)
        {
            _dialogs.Notify(
                $"{failedPaths.Count} arquivo(s) não puderam ser excluídos (arquivos >2GB, em rede ou em uso não vão para a Lixeira).",
                "Exclusões com falha");
        }
    }

    private void ApplySort()
    {
        using (ItemsView.DeferRefresh())
        {
            ItemsView.SortDescriptions.Clear();
            foreach (var sort in SortDescriptionsFor(SelectedSort))
                ItemsView.SortDescriptions.Add(sort);
        }
    }

    private static IEnumerable<SortDescription> SortDescriptionsFor(SortMode mode) => mode switch
    {
        SortMode.DateDescending => [new(nameof(ImageItemViewModel.LastModified), ListSortDirection.Descending)],
        SortMode.DateAscending => [new(nameof(ImageItemViewModel.LastModified), ListSortDirection.Ascending)],
        SortMode.SizeDescending => [new(nameof(ImageItemViewModel.SizeBytes), ListSortDirection.Descending)],
        SortMode.SizeAscending => [new(nameof(ImageItemViewModel.SizeBytes), ListSortDirection.Ascending)],
        SortMode.NameAscending => [new(nameof(ImageItemViewModel.FileName), ListSortDirection.Ascending)],
        _ => [],
    };

    private void AddGroup(DuplicateGroupViewModel group)
    {
        _groups.Add(group);
        foreach (var item in group.Items)
        {
            item.PropertyChanged += OnItemPropertyChanged;
            Items.Add(item);
        }
    }

    private void RemoveItem(ImageItemViewModel item)
    {
        item.PropertyChanged -= OnItemPropertyChanged;
        item.IsSelected = false;
        Items.Remove(item);
        item.Group?.Items.Remove(item);
    }

    private void PruneEmptyGroups()
    {
        // A group with a single (or zero) remaining file is no longer a duplicate set; drop it entirely.
        foreach (var group in _groups.Where(g => g.Items.Count <= 1).ToList())
        {
            foreach (var item in group.Items.ToList())
                RemoveItem(item);
            _groups.Remove(group);
        }
        RecountSelected();
    }

    private void ClearGroups()
    {
        foreach (var item in Items)
            item.PropertyChanged -= OnItemPropertyChanged;
        Items.Clear();
        _groups.Clear();
        SelectedCount = 0;
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ImageItemViewModel.IsSelected))
            RecountSelected();
    }

    private void RecountSelected() => SelectedCount = Items.Count(i => i.IsSelected);

    public void Dispose() => _scanCts?.Dispose();
}
