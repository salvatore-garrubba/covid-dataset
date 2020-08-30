using Covid.Importer.Domain.Models;
using TinyCsvParser.Mapping;

namespace Covid.Importer.Mapping
{
    public class CsvInputDataMapping : CsvMapping<InputData> //ICsvMapping<InputData>
    {
        public CsvInputDataMapping()
            : base()
        {
            //id|category|question|answer|datetime
            MapProperty(0, x => x.QuestionId);
            MapProperty(1, x => x.CategoryId);
            MapProperty(2, x => x.QuestionText);
            MapProperty(3, x => x.AnswerText);
            MapProperty(4, x => x.Timestamp);
        }
    }
}