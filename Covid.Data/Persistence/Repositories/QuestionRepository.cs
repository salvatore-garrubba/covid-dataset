using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using Covid.Data.Domain.Models;
using Covid.Data.Domain.Repositories;
using Covid.Data.Persistence.Contexts;

namespace Covid.Data.Persistence.Repositories
{
    public class QuestionRepository : BaseRepository, IQuestionRepository
    {
        public QuestionRepository(CovidContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Question>> GetTop10Async()
        {
            var result = await _context.Questions
            .Select(question => new
            {
                OriginalQuestion = question,
                AnswerCount = question.Answers.Count
            }).OrderByDescending(q => q.AnswerCount).Take(10).ToListAsync();
            return result.Select(q => q.OriginalQuestion);                        
        }
    }
}
