using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Live;

namespace CloudBackup.Files
{
    class AzureFileStorageProvider : IFileProvider, IFileStorage
    {
        private readonly LiveConnectClient _client;

        public AzureFileStorageProvider(LiveConnectClient client)
        {
            _client = client;
        }

        public async Task<Stream> ReadFileAsync(IFile file)
        {
            var azureFile = file as AzureFile;
            if (azureFile == null) return null;

            string url = azureFile.Data["source"] as string;
            if (string.IsNullOrEmpty(url)) return null;
            
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        }

        public Task<IPager<IFile>> GetFiles(string folder = null)
        {
            if (folder == null)
            {
                folder = "me/skydrive/camera_roll";
            }

            IPager<IFile> pager = new AzureFilePager(_client, folder);

            return Task.FromResult(pager);
        }

        public Task SaveFileAsync(string fileName, Stream data)
        {
            throw new NotImplementedException();
        }
    }
}
