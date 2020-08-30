using System.Collections.Generic;
using System.Threading.Tasks;
using Covid.Importer.Domain.Models;

namespace Covid.Importer.Persistence
{
    public interface IDBPersister
    {
        Task<ImportResult> PersistAsync(IList<ConvertedData> aggregatedModelItems);
    }
}