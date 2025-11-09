using Avalonia.Controls;
using Avalonia.Interactivity;
using MusicParserDesktop.ViewModels;

namespace MusicParserDesktop.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private async void ParseButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                await vm.ParsePlaylist();
            }
        }
    }
}