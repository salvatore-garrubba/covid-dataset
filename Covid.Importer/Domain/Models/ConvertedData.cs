using System;
using Covid.Data.Domain.Models;

namespace Covid.Importer.Domain.Models
{
    public class ConvertedData
    {
        public string FileName {get; set;}
        public Answer Answer {get; set;}
        public Question Question {get; set;}
        public Category Category {get; set;}

    }
}