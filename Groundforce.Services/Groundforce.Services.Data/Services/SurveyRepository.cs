using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public class SurveyRepository : ISurveyRepository
    {
        private readonly AppDbContext _ctx;
        public int TotalNumberOfItems { get; set; }

        public SurveyRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<UserSurvey>> GetUserSurveyByAgentIdandByStatus(string agentId, string status)
        {
            return await _ctx.UserSurveys.Where(x => x.ApplicationUserId == agentId && x.Status == status).Include(x => x.Survey).Include(x => x.Survey.SurveyType).ToListAsync();
        }

        public async Task<IEnumerable<UserSurvey>> GetUserSurveysByAgentIdandByStatusPaginated(string agentId, string status, int page, int per_page)
        {
            var items = await GetUserSurveyByAgentIdandByStatus(agentId, status);
            TotalNumberOfItems = items.Count();
            var pagedItems = items.Skip((page - 1) * per_page).Take(per_page).ToList(); 
            return pagedItems;
        }

        public async Task<UserSurvey> GetUserSurveyByUserIdAndSurveyId(string userId, string surveyId)
        {
            return await _ctx.UserSurveys.Where(x => x.ApplicationUserId == userId && x.SurveyId == surveyId).FirstOrDefaultAsync();
        }

        public async Task<QuestionOption> GetQuestionOptionById(string Id)
        {
            return await _ctx.QuestionOptions.FirstOrDefaultAsync(x => x.QuestionOptionId == Id);
        } 

        public async Task<SurveyType> GetSurveyTypeById(string Id)
        {
            return await _ctx.SurveyTypes.FirstOrDefaultAsync(x => x.SurveyTypeId == Id);
        }

        public async Task<Response> GetResponseByResponseId(string Id)
        {
            return await _ctx.Responses.FirstOrDefaultAsync(x => x.ResponseId == Id);
        }

        public async Task<Survey> GetSurveyById(string Id)
        {
            return await _ctx.Surveys.FirstOrDefaultAsync(x => x.SurveyId == Id);
        }

        public async Task<SurveyToReturnDTO> GetSurveyQuestionsBySurveyId(string Id)
        {
            var survey = await _ctx.Surveys.FirstOrDefaultAsync(x => x.SurveyId == Id);
            if (survey == null)
                throw new NullReferenceException("Null result found for survey");
            var questions = await _ctx.SurveyQuestions.Where(x => x.SurveyId == Id).Select(x => x.SurveyQuestionId).ToListAsync();

            if (questions == null)
                throw new NullReferenceException("Null result found for questions");

            var surveyQuestions = new SurveyToReturnDTO
            {
                SurveyId = Id,
                Topic = survey.Topic,
                Questions = questions
            };

            return surveyQuestions;

        }

        public async Task<SurveyQuestion> GetSurveyQuestionById(string Id)
        {
            return await _ctx.SurveyQuestions.Where(x => x.SurveyQuestionId == Id).Include(x => x.QuestionOptions).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<QuestionOption>> GetQuestionOptionsByQuestionId(string Id)
        {
            return await _ctx.QuestionOptions.Where(x => x.SurveyQuestionId == Id).ToListAsync();
        }

        public Response GetUserSurveyResponseByIds(string surveyId, string userId, string questionId)
        {
            return GetUserSurveyResponsesByUserAndSurveyIds(surveyId, userId).Result.FirstOrDefault(x => x.SurveyQuestionId == questionId);
        }

        public async Task<IEnumerable<Response>> GetUserSurveyResponsesByQuestionIdPaginated(string questionId, int page, int per_page)
        {
            var items = _ctx.Responses.Where(x => x.SurveyQuestionId == questionId);
            TotalNumberOfItems = items.Count();
            var pagedItems = await items.Skip((page - 1) * per_page).Take(per_page).ToListAsync();
            return pagedItems;
        }

        public async Task<IEnumerable<Response>> GetUserSurveyResponsesByUserAndSurveyIds(string surveyId, string userId)
        {
            return await _ctx.Responses.Where(x => x.SurveyId == surveyId && x.ApplicationUserId == userId).ToListAsync();
        }

       
        public async Task<IEnumerable<SurveyQuestion>> GetAllSurveyQuestionsAndSurveyOptions()
        {
            return await _ctx.SurveyQuestions.Include(x => x.QuestionOptions).ToListAsync();
        }

        public async Task<IEnumerable<SurveyQuestion>> GetAllSurveyQuestionsAndSurveyOptionsPaginated(int page, int per_page)
        {
            var items = await GetAllSurveyQuestionsAndSurveyOptions();
            TotalNumberOfItems = items.Count();
            var pagedItems = items.Skip((page - 1) * per_page).Take(per_page).ToList();
            return pagedItems;
        }
    }
}
