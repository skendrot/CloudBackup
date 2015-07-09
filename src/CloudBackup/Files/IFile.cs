using System.IO;
using System.Threading.Tasks;

namespace CloudBackup.Files
{
    interface IFile
    {
        string Name { get; }
    }
}
