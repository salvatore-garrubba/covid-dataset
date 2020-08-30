using System;

namespace Covid.Data.Domain.Models
{
    public class Answer
    {
        public string Id {get; set;}

        public int QuestionId {get; set;}
        public Question Question {get; set;}
        public DateTime Timestamp {get; set;}

        public string Text {get; set;}
    }
}