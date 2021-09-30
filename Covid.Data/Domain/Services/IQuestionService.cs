using System.Collections.Generic;
using System.Threading.Tasks;
using Covid.Data.Domain.Models;

namespace Covid.Data.Domain.Services
{
    public interface IQuestionService
    {
        Task<IEnumerable<Question>> GetTop10Async();
    }
}
