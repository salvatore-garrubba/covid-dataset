using System;
using System.Collections.Generic;

namespace Covid.Importer.Domain.Models
{
    public class ImportResult
    {        
        public int Batch {get; set;}
        public IEnumerable<ConvertedData> Data {get; set;}

        public bool HasError{get; set;}

        //public string ErrorMessage {get; set;}

        public Exception OccuredException {get; set;}
    }
}