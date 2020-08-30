using System.Collections.Generic;

namespace Covid.Data.Domain.Models
{
    public class Question
    {
         public int Id { get; set; }

         public int CategoryId {get; set;}

         public Category Category {get; set;}

         public string Text {get; set;}

         public ICollection<Answer> Answers {get; set;}
    }
}