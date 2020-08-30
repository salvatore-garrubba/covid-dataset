using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AutoMapper;
using Covid.Importer.Domain.Models;
using Covid.Importer.Mapping;
using Microsoft.Extensions.Logging;
using TinyCsvParser;

namespace Covid.Importer.Services
{

    public class DecoderService : IDecoderService
    {
        private readonly ILogger<DecoderService> _logger;
        private readonly IMapper _mapper;
        private readonly CsvParser<InputData> _csvParser;

        private readonly CsvReaderOptions _csvReaderOptions;

        // public DecoderService()
        // {
        // }

        public DecoderService(ILogger<DecoderService> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            CsvParserOptions csvParserOptions = new CsvParserOptions(false, '|');
            CsvInputDataMapping csvMapper = new CsvInputDataMapping();
            _csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            //TODO pensare se fare DI introducendo interfaccia ICSParser
            _csvParser = new CsvParser<InputData>(csvParserOptions, csvMapper);
        }

        public string CSVLine(InputData inputData, string errorMessage)
        {
            StringBuilder sb = new StringBuilder();
            string sep = "|";
            IFormatProvider fp = CultureInfo.InvariantCulture;
            if (inputData != null)
            {
                //id|category|question|answer|datetime|fileName|errormessage
                
                sb.Append(inputData.QuestionId);
                sb.Append(sep);
                sb.Append(inputData.CategoryId);
                sb.Append(sep);
                sb.Append(inputData.QuestionText);
                sb.Append(sep);
                sb.Append(inputData.AnswerText);
                sb.Append(sep);
                sb.Append(inputData.Timestamp.ToString("u", fp));
                sb.Append(sep);
                sb.Append(inputData.FileName);
                sb.Append(sep);
                sb.Append(errorMessage); 
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public ConvertedData Decode(InputData inputData)
        {
            return _mapper.Map<ConvertedData>(inputData);
        }

        public InputData Encode(ConvertedData convertedData)
        {
            return _mapper.Map<InputData>(convertedData);
        }
        public InputData Parse(string inputLine)
        {
            InputData result = null;
            if (inputLine != null)
            {
                var cm = _csvParser.ReadFromString(_csvReaderOptions, inputLine).ToList();
                result = cm.First().Result;                
            }
            return result;
        }

        public IEnumerable<InputData> ParseAll(string input)
        {
            var result = _csvParser.ReadFromString(_csvReaderOptions, input).ToList();
            int tot = result.Count;
            return result.Select(x => x.Result);
        }
    }
}