using System;

namespace Covid.Importer.Domain.Models
{
    public class InputData
    {
        public string FileName {get; set;}
        public int QuestionId {get; set;}
        public int CategoryId {get; set;}

        public string QuestionText {get; set;} 

        public string AnswerText {get; set;}

        public DateTime Timestamp {get; set;} 
    }
}