using System.Threading.Tasks;
using Covid.Importer.Domain.Models;

namespace Covid.Importer.Persistence
{
    public interface IErrorPersister
    {
        Task PersistErrorsAsync(ImportResult importResult); 
    }
}