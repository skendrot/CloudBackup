using System.Threading.Tasks;
using Microsoft.Live;

namespace CloudBackup
{
    internal class RefeshTokenHandler : IRefreshTokenHandler
    {
        public Task SaveRefreshTokenAsync(RefreshTokenInfo tokenInfo)
        {
            Properties.Settings.Default.RefreshToken = tokenInfo.RefreshToken;
            Properties.Settings.Default.Save();
            return Task.FromResult(true);
        }

        public Task<RefreshTokenInfo> RetrieveRefreshTokenAsync()
        {
            RefreshTokenInfo tokenInfo = null;
            var token = Properties.Settings.Default.RefreshToken;
            if (string.IsNullOrEmpty(token))
            {
                return Task.FromResult(tokenInfo);
            }

            tokenInfo = new RefreshTokenInfo(token);
            return Task.FromResult(tokenInfo);
        }
    }
}