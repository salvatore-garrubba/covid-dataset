using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

using Covid.Importer.Domain.Models;

namespace Covid.Importer.Services
{
    public interface IDecoderService
    {
        ConvertedData Decode(InputData inputData);
        InputData Encode(ConvertedData convertedData);
        string CSVLine(InputData inputData, string errorMessage);
        InputData Parse(string inputLine);

        IEnumerable<InputData> ParseAll(string input);

    }
}