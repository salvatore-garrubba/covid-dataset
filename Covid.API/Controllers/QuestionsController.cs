using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Covid.API.Domain.Models;
using Covid.Data.Domain.Models;
using Covid.Data.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Covid.API.Controllers
{
    //https://localhost:5001/api/questions
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        
        private readonly ILogger<QuestionsController> _logger;

        private readonly IMapper _mapper;

        public QuestionsController(ILogger<QuestionsController> logger, IQuestionService questionService, IMapper mapper)
        {
            _logger = logger;
            _questionService = questionService;
            _mapper = mapper;            
        }
        
        [HttpGet]
        public async Task<IEnumerable<QuestionResource>> GetTop10Async()
        {            
            var questions = await _questionService.GetTop10Async();
            
            return _mapper.Map<IEnumerable<Question>, IEnumerable<QuestionResource>>(questions);
        }
        
    }
}