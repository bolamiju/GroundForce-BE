using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface ISurveyRepository
    {
        int TotalNumberOfItems { get; set; }

        Task<IEnumerable<UserSurvey>> GetUserSurveyByAgentIdandByStatus(string agentId, string status);
        Task<IEnumerable<UserSurvey>> GetUserSurveysByAgentIdandByStatusPaginated(string agentId, string status, int page, int per_page);
        Task<UserSurvey> GetUserSurveyByUserIdAndSurveyId(string userId, string surveyId);
        Task<QuestionOption> GetQuestionOptionById(string Id); 
        Task<SurveyType> GetSurveyTypeById(string Id);
        Task<Response> GetResponseByResponseId(string Id);

        Task<Survey> GetSurveyById(string Id);

        Task<SurveyQuestion> GetSurveyQuestionById(string Id);
        Task<IEnumerable<QuestionOption>> GetQuestionOptionsByQuestionId(string Id);

        Response GetUserSurveyResponseByIds(string surveyId, string userId, string questionId);

        Task<SurveyToReturnDTO> GetSurveyQuestionsBySurveyId(string Id);

        Task<IEnumerable<Response>> GetUserSurveyResponsesByQuestionIdPaginated(string questionId, int page, int per_page);

        Task<IEnumerable<Response>> GetUserSurveyResponsesByUserAndSurveyIds(string surveyId, string userId);
        Task<IEnumerable<SurveyQuestion>> GetAllSurveyQuestionsAndSurveyOptions();
        Task<IEnumerable<SurveyQuestion>> GetAllSurveyQuestionsAndSurveyOptionsPaginated(int page, int per_page);
    }
}
