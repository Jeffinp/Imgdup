using System.Windows;
using System.Windows.Controls;
using Imgdup.App.ViewModels;

namespace Imgdup.App;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }

    /// <summary>
    /// Triggers lazy thumbnail decoding when a gallery item's container is realized (e.g. scrolled into view).
    /// With UI virtualization, only visible items decode, keeping memory and CPU bounded.
    /// </summary>
    private async void OnItemLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: ImageItemViewModel item })
            await item.EnsureThumbnailAsync(MainViewModel.ThumbnailDecodePixels);
    }
}
