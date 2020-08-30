
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Covid.Data.Domain.Models
{
    public class Category
    {
         //[Key, Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
         public int Id { get; set; }
         public ICollection<Question> Questions {get; set;}
    }
}