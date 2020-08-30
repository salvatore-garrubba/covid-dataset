using System.Collections.Generic;
using System.Threading.Tasks;
using Covid.Data.Domain.Models;
using Covid.Data.Domain.Repositories;

namespace Covid.Data.Domain.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionService(IQuestionRepository questionRepository)
        {
            this._questionRepository = questionRepository;
        }

        public async Task<IEnumerable<Question>> GetTop10Async()
        {
            return await this._questionRepository.GetTop10Async();
        }

    }
}