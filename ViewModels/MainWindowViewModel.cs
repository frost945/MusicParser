using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MusicParserDesktop.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _url = string.Empty;
        private Playlist _playlist;
        private bool _isLoading;
        private string _errorMessage = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Url
        {
            get => _url;
            set => SetProperty(ref _url, value);
        }

        public Playlist Playlist
        {
            get => _playlist;
            set
            {
                SetProperty(ref _playlist, value);
                OnPropertyChanged(nameof(HasPlaylist));
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                SetProperty(ref _isLoading, value);
                OnPropertyChanged(nameof(IsNotLoading));
            }
        }

        public bool IsNotLoading => !IsLoading;
        public bool HasPlaylist => Playlist?.Songs?.Any() == true;

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value);
                OnPropertyChanged(nameof(HasError));
            }
        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public MainWindowViewModel()
        {
            Playlist = new Playlist();
        }

        private string _statusMessage = string.Empty;

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public async Task ParsePlaylist()
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                ShowError("Please enter a URL");
                return;
            }

            try
            {
                ClearResults();

                IsLoading = true;
                ClearError();
                StatusMessage = "Parsing started...";

                var parserService = new ParserService();
                Playlist = await parserService.ParseHtmlAsync(Url);

                StatusMessage = "Parsing completed successfully";
            }
            
            catch (Exception ex)
            {
                ShowError($"Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ClearResults()
        {
            Playlist = new Playlist();

            ClearError();

            // Force updating bindings
            OnPropertyChanged(nameof(HasPlaylist));
        }

        private void ShowError(string message)
        {
            ErrorMessage = message;
            StatusMessage = string.Empty;
        }

        private void ClearError()
        {
            ErrorMessage = string.Empty;
            StatusMessage = string.Empty;
        }
        

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}