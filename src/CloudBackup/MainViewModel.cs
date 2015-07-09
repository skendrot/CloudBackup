using Microsoft.Live;
using CloudBackup.Annotations;
using CloudBackup.Files;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CloudBackup
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private LiveConnectClient _client;
        private bool _loggingIn;
        private bool _loading;
        private LiveOperationResult _cameraRollFolder;
        private bool _isDownloading;
        private int _itemsDownloaded;

        public MainViewModel()
        {
            DownloadCommand = new DelegateCommand(Download, CanDownload);
        }

        public ICommand DownloadCommand { get; private set; }

        public LiveOperationResult CameraRollFolder
        {
            get { return _cameraRollFolder; }
            set
            {
                if (Equals(value, _cameraRollFolder)) return;
                _cameraRollFolder = value;
                OnPropertyChanged();
            }
        }

        public bool Loading
        {
            get { return _loading; }
            set
            {
                if (value.Equals(_loading)) return;
                _loading = value;
                OnPropertyChanged();
            }
        }

        public bool IsDownloading
        {
            get { return _isDownloading; }
            private set
            {
                if (value.Equals(_isDownloading)) return;
                _isDownloading = value;
                OnPropertyChanged();
            }
        }

        public bool LoggingIn
        {
            get { return _loggingIn; }
            set
            {
                if (value.Equals(_loggingIn)) return;
                _loggingIn = value;
                OnPropertyChanged();
            }
        }

        public int ItemsDownloaded
        {
            get { return _itemsDownloaded; }
            private set
            {
                if (value == _itemsDownloaded) return;
                _itemsDownloaded = value;
                OnPropertyChanged();
            }
        }

        public async Task Login()
        {
            LoggingIn = true;

            var auth = new Authenticator("0000000048122D4E", new RefeshTokenHandler());
            var session = await auth.GetSession();
            LoggingIn = false;

            Loading = true;
            if (session != null)
            {
                _client = new LiveConnectClient(session);
                CameraRollFolder = await _client.GetAsync("me/skydrive/camera_roll");
                ((DelegateCommand)DownloadCommand).RaiseCanExecuteChanged();
            }

            Loading = false;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool CanDownload(object obj)
        {
            return CameraRollFolder != null;
        }

        private async void Download(object obj)
        {
            // TODO: Provide a picker to select from/to locations
            var fileStorage = new DiskFileStorage("c:\\backup");
            IFileProvider provider = new AzureFileStorageProvider(_client);

            ItemsDownloaded = 0;
            IsDownloading = true;

            // TODO: Show folders to the user to select a single folder
            var pager = await provider.GetFiles("me/skydrive/camera_roll");

            while (pager.HasMoreItems)
            {
                IEnumerable<IFile> files = await pager.GetNextItemsAsync();
                if (files != null)
                {
                    foreach (IFile file in files)
                    {
                        ItemsDownloaded++;

                        using (var imageStream = await provider.ReadFileAsync(file))
                        {
                            await fileStorage.SaveFileAsync(file.Name, imageStream);
                        }
                    }
                }
            }
            IsDownloading = false;
        }
    }
}
