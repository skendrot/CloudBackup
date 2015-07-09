using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CloudBackup.Files
{
    interface IFileProvider
    {
        Task<Stream> ReadFileAsync(IFile file);
        Task<IPager<IFile>> GetFiles(string folder = null);
    }
}