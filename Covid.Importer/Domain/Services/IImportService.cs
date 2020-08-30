using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

using Covid.Importer.Domain.Models;

namespace Covid.Importer.Services
{
    public interface IImportService //ProcessingPipeline
    {
         Task StartImportAsync(CancellationToken token);
    }

    

}