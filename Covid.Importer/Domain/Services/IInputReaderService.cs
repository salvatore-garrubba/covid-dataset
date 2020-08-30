using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Covid.Importer.Domain.Models;

namespace Covid.Importer.Services
{
    public interface IInputReaderService
    {
        Task StartConsumingAsync(ITargetBlock<InputData> target, CancellationToken token);
    }
}