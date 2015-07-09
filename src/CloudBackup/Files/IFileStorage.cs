using System.IO;
using System.Threading.Tasks;

namespace CloudBackup.Files
{
    interface IFileStorage
    {
        Task SaveFileAsync(string fileName, Stream data);
    }
}