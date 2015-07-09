using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudBackup.Files
{
    class AzureFile : IFile
    {
        public AzureFile(IDictionary<string, object> data)
        {
            Data = data;
        }

        public string Name
        {
            get
            {
                if (Data.ContainsKey("name"))
                {
                    return Data["name"] as string;
                }
                return null;
            }
        }

        internal IDictionary<string, object> Data { get; private set; }
    }
}
