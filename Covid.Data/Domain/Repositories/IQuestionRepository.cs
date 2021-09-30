using System.Collections.Generic;
using System.Threading.Tasks;
using Covid.Data.Domain.Models;

namespace Covid.Data.Domain.Repositories
{
    public interface IQuestionRepository
    {        
        Task<IEnumerable<Question>> GetTop10Async();
    }  
}
    
