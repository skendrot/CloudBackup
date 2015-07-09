using System.IO;
using System.Threading.Tasks;

namespace CloudBackup.Files
{
    class DiskFileStorage : IFileStorage
    {
        private readonly string _rootDir;

        public DiskFileStorage(string rootDir)
        {
            _rootDir = rootDir;
            if (Directory.Exists(rootDir) == false)
            {
                Directory.CreateDirectory(rootDir);
            }
        }

        public Task SaveFileAsync(string fileName, Stream data)
        {
            fileName = Path.Combine(_rootDir, fileName);
            if (File.Exists(fileName) == false)
            {
                using (var fileStream = File.Create(fileName))
                {
                    data.CopyTo(fileStream);
                }
            }
            return Task.FromResult(true);
        }
    }
}
