using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Live;

namespace CloudBackup
{
    class Authenticator
    {
        private readonly string _clientId;
        private readonly IRefreshTokenHandler _tokenHandler;
        private LiveAuthClient _client;

        public Authenticator(string clientId, IRefreshTokenHandler tokenHandler)
        {
            _clientId = clientId;
            _tokenHandler = tokenHandler;
        }

        public async Task<LiveConnectSession> GetSession()
        {
            _client = new LiveAuthClient(_clientId, _tokenHandler);

            var existingToken = await _tokenHandler.RetrieveRefreshTokenAsync();
            if (existingToken != null)
            {
                var loginResult = await _client.InitializeAsync();
                return loginResult.Session;
            }
            return await LoginAsync();
        }

        private Task<LiveConnectSession> LoginAsync()
        {
            TaskCompletionSource<LiveConnectSession> taskCompletion = new TaskCompletionSource<LiveConnectSession>();

            var url = _client.GetLoginUrl(new[] { "wl.basic", "wl.signin", "onedrive.readonly", "wl.skydrive", "wl.photos" });

            WebBrowser browser = new WebBrowser();
            var window = new Window();
            window.Content = browser;

            NavigatingCancelEventHandler handler = null;
            handler = async (o, args) =>
            {
                if (args.Uri.AbsolutePath.Equals("/oauth20_desktop.srf"))
                {
                    browser.Navigating -= handler;
                    window.Close();

                    var session = await GetConnectSession(args.Uri.Query);
                    taskCompletion.SetResult(session);
                }
            };
            browser.Navigating += handler;
            browser.Navigate(url);
            window.Show();
            return taskCompletion.Task;
        }

        private async Task<LiveConnectSession> GetConnectSession(string response)
        {
            response = response.TrimStart('?');
            var parts = response.Split('&');

            foreach (var part in parts)
            {
                var values = part.Split('=');
                var key = values[0];
                var value = values[1];
                if (key == "code")
                {
                    return await _client.ExchangeAuthCodeAsync(value);
                }
            }
            return null;
        }
    }
}
