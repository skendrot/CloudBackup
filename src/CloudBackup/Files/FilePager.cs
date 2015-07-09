using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudBackup.Files
{
    interface IPager<T>
    {
        Task<IEnumerable<T>> GetNextItemsAsync();
        bool HasMoreItems { get; }
    }
}
