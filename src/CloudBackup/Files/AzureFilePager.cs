using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Live;

namespace CloudBackup.Files
{
    class AzureFilePager : IPager<IFile>
    {
        private readonly LiveConnectClient _client;
        private string _nextFiles;

        public AzureFilePager(LiveConnectClient client, string folderId)
        {
            _client = client;
            _nextFiles = string.Format("{0}/files?limit=100&offset=0", folderId);
        }

        public async Task<IEnumerable<IFile>> GetNextItemsAsync()
        {
            List<IFile> files = new List<IFile>();
            var response = await _client.GetAsync(_nextFiles).ConfigureAwait(false);
            if (response.Result.ContainsKey("data"))
            {
                var data = response.Result["data"] as IList<object>;
                if (data != null)
                {
                    files.AddRange(data.OfType<IDictionary<string, object>>().Select(t => new AzureFile(t)));
                }
            }

            _nextFiles = null;
            if (response.Result.ContainsKey("paging"))
            {
                var paging = response.Result["paging"] as IDictionary<string, object>;
                if (paging != null && paging.ContainsKey("next"))
                {
                    _nextFiles = paging["next"] as string;
                }
            }
            return files;
        }

        public bool HasMoreItems
        {
            get { return string.IsNullOrEmpty(_nextFiles) == false; }
        }
    }
}
