using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Groundforce.Common.Utilities.Helpers;
using Groundforce.Services.Data.Services;
using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Groundforce.Services.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SurveyController : ControllerBase
    {
        private readonly ILogger<SurveyController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISurveyRepository _surveyRepository;
        private readonly IAllRepo<UserSurvey> _userSurveyRepository;
        private readonly IAllRepo<QuestionOption> _questionOptionRepository;
        private readonly IAllRepo<SurveyType> _surveyTypeRepository;
        private readonly IAllRepo<Survey> _surveyCrudRepo;
        private readonly IAllRepo<SurveyQuestion> _surveyQuestionCrudRepo;
        private readonly IAllRepo<Response> _responseCrudRepo;
        private readonly IAgentRepository _agentRepository;
        private readonly int perPage; 

        public SurveyController(ILogger<SurveyController> logger, UserManager<ApplicationUser> userManager,
            ISurveyRepository surveyRepository, IAllRepo<UserSurvey> userSurveyRepository, IAllRepo<QuestionOption> questionOptionRepository,
            IAllRepo<SurveyType> surveyTypeRepository, IAllRepo<Survey> surveyCrudRepo,
            IAllRepo<SurveyQuestion> surveyQuestionCrudRepo, IAllRepo<Response> responseCrudRepo,
            IAgentRepository agentRepository, IConfiguration configuration)
        {
            _logger = logger;
            _userManager = userManager;
            _surveyRepository = surveyRepository;
            _userSurveyRepository = userSurveyRepository;
            _questionOptionRepository = questionOptionRepository;
            _surveyTypeRepository = surveyTypeRepository;
            _surveyCrudRepo = surveyCrudRepo;
            _surveyQuestionCrudRepo = surveyQuestionCrudRepo;
            _responseCrudRepo = responseCrudRepo;
            _agentRepository = agentRepository;
            perPage = Convert.ToInt32(configuration.GetSection("PaginationSettings:PerPage").Get<string>()); 
        }

        #region SURVEY TYPE ROUTE

        [Authorize(Roles = "admin")]
        [HttpPost("add-survey-type")]
        public async Task<IActionResult> AddSurveyType([FromBody] SurveyTypeDTO model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Could not access user" }));

            // generate survey type id
            string surveyTypeId;
            SurveyType result;
            do
            {
                surveyTypeId = Guid.NewGuid().ToString();
                result = await _surveyRepository.GetSurveyTypeById(surveyTypeId);
            } while (result != null);

            var surveyType = new SurveyType
            {
                SurveyTypeId = surveyTypeId,
                Type = model.Type,
                AddedBy = user.Id,
                UpdatedBy = user.Id
            };

            try
            {
                await _surveyTypeRepository.Add(surveyType);
                return Ok(ResponseMessage.Message("Success", data: new { id = surveyType.SurveyTypeId }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to add survey type" }));
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("edit-survey-type")]
        public async Task<IActionResult> EditSurveyType([FromBody] UpdateSurveyTypeDTO model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Could not access user" }));

            SurveyType surveyType;

            try
            {
                surveyType = await _surveyRepository.GetSurveyTypeById(model.TypeId);
                if (surveyType == null)
                    return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Survey Type does not exist" }));
            }
            catch (DbException de)
            {
                _logger.LogError(de.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }

            surveyType.Type = model.Type;
            surveyType.UpdatedAt = DateTime.Now;
            surveyType.UpdatedBy = user.Id;

            try
            {
                await _surveyTypeRepository.Update(surveyType);
                return Ok(ResponseMessage.Message("Success", data: new { message = "Survey Type Updated Successfully" }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to update survey type" }));
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("delete-survey-type/{typeId}")]
        public async Task<IActionResult> DeleteSurveyType(string typeId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Could not access user" }));

            SurveyType surveyType;
            try
            {
                surveyType = await _surveyRepository.GetSurveyTypeById(typeId);
                if (surveyType == null)
                    return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Survey type does not exist" }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }

            try
            {
                await _surveyTypeRepository.Delete(surveyType);
                return Ok(ResponseMessage.Message("Success", data: new { message = "Survey type deleted!!!" }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Survey type could not be deleted" }));
            }
        }
        #endregion

        #region SURVEY ROUTE
        [Authorize(Roles = "admin")]
        [HttpPost("add-survey")]
        public async Task<IActionResult> AddSurvey([FromBody] SurveyDTO surveyToAdd)
        {
            if (ModelState.IsValid)
            {
                var LoggedInUser = await _userManager.GetUserAsync(User);

                if (LoggedInUser == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Can't access loggedIn user" }));

                // generate survey id
                string surveyId;
                Survey result;
                do
                {
                    surveyId = Guid.NewGuid().ToString();
                    result = await _surveyRepository.GetSurveyById(surveyId);
                } while (result != null);

                // construct survey object
                var newSurvey = new Survey
                {
                    SurveyId = surveyId,
                    AddedBy = LoggedInUser.Id,
                    UpdatedBy = LoggedInUser.Id,
                    SurveyTypeId = surveyToAdd.SurveyTypeId,
                    Topic = surveyToAdd.Topic
                };

                //add survey to database
                try
                {
                    await _surveyCrudRepo.Add(newSurvey);
                    return Ok(ResponseMessage.Message("Survey added!", data: new { id = newSurvey.SurveyId })); 
                }
                catch (DbException de)
                {
                    _logger.LogError(de.Message);
                    return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
                }
            }

            return BadRequest(ResponseMessage.Message("Bad Request", errors: new { message = "Invalid model state", ModelState }));
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("edit-survey")]
        public async Task<IActionResult> EditSurvey([FromBody] UpdateSurveyDTO surveyToUpdate)
        {
            if (ModelState.IsValid)
            {
                if (String.IsNullOrWhiteSpace(surveyToUpdate.SurveyId) || String.IsNullOrWhiteSpace(surveyToUpdate.SurveyTypeId))
                    return BadRequest(ResponseMessage.Message("Invalid Id", errors: new { message = "Survey Id should not be null or empty or whitespace" }));

                var loggedInUser = await _userManager.GetUserAsync(User);

                if (loggedInUser == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Can't access loggedIn user" }));

                try
                {
                    var surveyType = await _surveyRepository.GetSurveyTypeById(surveyToUpdate.SurveyTypeId);
                    if (surveyType == null)
                        return NotFound(ResponseMessage.Message("Not found", errors: new { message = $"Invalid survey type id: {surveyToUpdate.SurveyId}" }));
                }
                catch (DbException de)
                {
                    _logger.LogError(de.Message);
                    return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
                }

                Survey survey;
                try
                {
                    survey = await _surveyRepository.GetSurveyById(surveyToUpdate.SurveyId);
                    if (survey == null)
                        return NotFound(ResponseMessage.Message("Not found", errors: new { message = $"Survey with id: {surveyToUpdate.SurveyId} was not found" }));
                }
                catch (DbException de)
                {
                    _logger.LogError(de.Message);
                    return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
                }

                survey.SurveyId = surveyToUpdate.SurveyId;
                survey.UpdatedBy = loggedInUser.Id;
                survey.SurveyTypeId = surveyToUpdate.SurveyTypeId;
                survey.Topic = surveyToUpdate.Topic;
                survey.UpdatedAt = DateTime.Now;

                try
                {
                    await _surveyCrudRepo.Update(survey);
                    return Ok(ResponseMessage.Message("Success", data: new { message = "Survey updated" }));
                }
                catch (DbException de)
                {
                    _logger.LogError(de.Message);
                    return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
                }
            }
            return BadRequest(ResponseMessage.Message("Invalid model state", errors: ModelState));
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{surveyId}/delete-survey")]
        public async Task<IActionResult> DeleteSurvey(string surveyId)
        {
            if (string.IsNullOrWhiteSpace(surveyId))
                return BadRequest(ResponseMessage.Message("Invalid Id", errors: new { message = "Survey Id should not be null or empty or whitespace" }));

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Could not access user" }));

            Survey survey;
            try
            {
                survey = await _surveyRepository.GetSurveyById(surveyId);
                if (survey == null)
                    return NotFound(ResponseMessage.Message("Not found", errors: new { message = $"Survey with id: {surveyId} was not found" }));
            }
            catch (DbException de)
            {
                _logger.LogError(de.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }

            try
            {
                await _surveyCrudRepo.Delete(survey);
                return Ok(ResponseMessage.Message("Success", data: new { message = "Survey deleted!!!" }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad Request", errors: new { message = "Failed to delete survey!" }));
            }
        }
        #endregion

        #region SURVEY QUESTION ROUTE
        [Authorize(Roles = "admin")]
        [HttpPost("add-question")]
        public async Task<IActionResult> AddSurveyQuestion([FromBody] SurveyQuestionDTO model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.SurveyId))
                    return BadRequest(ResponseMessage.Message("Invalid Id", errors: new { message = "Survey Id should not be null or empty or whitespace" }));

                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Could not access user" }));

                // generate survey question id
                string surveyQuestionId;
                SurveyQuestion result;
                do
                {
                    surveyQuestionId = Guid.NewGuid().ToString();
                    result = await _surveyRepository.GetSurveyQuestionById(surveyQuestionId);
                } while (result != null);

                // construct survey question object
                var newSurveyQuestion = new SurveyQuestion
                {
                    SurveyQuestionId = surveyQuestionId,
                    Question = model.Question,
                    SurveyId = model.SurveyId,
                    AddedBy = user.Id,
                    UpdatedBy = user.Id
                };

                List<QuestionOption> options = new List<QuestionOption>();
                foreach (var option in model.Options)
                {
                    // generate option id
                    string questionOptionId;
                    QuestionOption questionOption;
                    do
                    {
                        questionOptionId = Guid.NewGuid().ToString();
                        questionOption = await _surveyRepository.GetQuestionOptionById(questionOptionId);
                    } while (questionOption != null);

                    var newQuestionOption = new QuestionOption
                    {
                        QuestionOptionId = questionOptionId,
                        Option = option,
                        SurveyQuestionId = surveyQuestionId
                    };

                    options.Add(newQuestionOption);
                }

                //add surveyQuestion to database
                try
                {
                    await _surveyQuestionCrudRepo.Add(newSurveyQuestion);
                    await _questionOptionRepository.AddRange(options);
                    return Ok(ResponseMessage.Message("Survey Question Added", data: new { id = newSurveyQuestion.SurveyQuestionId }));
                }
                catch (Exception de)
                {
                    _logger.LogError(de.Message);
                    return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
                }
            }

            return BadRequest(ResponseMessage.Message("Bad Request", errors: "Invalid model state", ModelState));
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("edit-question")]
        public async Task<IActionResult> EditSurveyQuestion([FromBody] UpdateSurveyQuestionDTO model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.QuestionId) || string.IsNullOrWhiteSpace(model.SurveyId))
                    return BadRequest(ResponseMessage.Message("Invalid Id", errors: new { message = "Survey Question and Survey Ids should not be null or empty or whitespace" }));

                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Could not access user" }));

                SurveyQuestion surveyQuestion;
                try
                {
                    surveyQuestion = await _surveyRepository.GetSurveyQuestionById(model.QuestionId);
                    if (surveyQuestion == null)
                        return NotFound(ResponseMessage.Message("Not found", errors: new { message = $"Survey Question with id: {model.QuestionId} was not found" }));

                    var optionsToDelete = _surveyRepository.GetQuestionOptionsByQuestionId(model.QuestionId).Result.ToList();
                    foreach (var optionToDelete in optionsToDelete)
                    {
                        await _questionOptionRepository.Delete(optionToDelete);
                    }

                    surveyQuestion.SurveyQuestionId = model.QuestionId;
                    surveyQuestion.SurveyId = model.SurveyId;
                    surveyQuestion.Question = model.Question;
                    surveyQuestion.UpdatedAt = DateTime.Now;
                    surveyQuestion.UpdatedBy = user.Id;

                    List<QuestionOption> options = new List<QuestionOption>();
                    foreach (var option in model.Options)
                    {
                        // generate option id
                        string questionOptionId;
                        QuestionOption questionOption;
                        do
                        {
                            questionOptionId = Guid.NewGuid().ToString();
                            questionOption = await _surveyRepository.GetQuestionOptionById(questionOptionId);
                        } while (questionOption != null);

                        var newQuestionOption = new QuestionOption
                        {
                            QuestionOptionId = questionOptionId,
                            Option = option,
                            SurveyQuestionId = model.QuestionId
                        };

                        options.Add(newQuestionOption);
                    }

                    await _surveyQuestionCrudRepo.Update(surveyQuestion);
                    await _questionOptionRepository.AddRange(options);
                    return Ok(ResponseMessage.Message("Success", data: new { message = "Survey Question Updated" }));
                }
                catch (Exception de)
                {
                    _logger.LogError(de.Message);
                    return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
                }
            }

            return BadRequest(ResponseMessage.Message("Bad Request", errors: new { message = "Invalid model state" }, ModelState));
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{questionId}/delete-question")]
        public async Task<IActionResult> DeleteSurveyQuestion(string questionId)
        {
            if (string.IsNullOrWhiteSpace(questionId))
                return BadRequest(ResponseMessage.Message("Invalid Id", errors: new { message = "Survey Question Id should not be null or empty or whitespace" }));

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Could not access user" }));

            SurveyQuestion surveyQuestion;
            try
            {
                surveyQuestion = await _surveyRepository.GetSurveyQuestionById(questionId);
                if (surveyQuestion == null)
                    return NotFound(ResponseMessage.Message("Not found", errors: new { message = $"Survey Question with id: {questionId} was not found" }));

                //get options and delete
                var optionsToDelete = _surveyRepository.GetQuestionOptionsByQuestionId(questionId).Result.ToList();
                foreach (var optionToDelete in optionsToDelete)
                {
                    await _questionOptionRepository.Delete(optionToDelete);
                }

                //delete question
                await _surveyQuestionCrudRepo.Delete(surveyQuestion);
                return Ok(ResponseMessage.Message("Success", data: new { message = "Survey question deleted!" }));
            }
            catch (Exception de)
            {
                _logger.LogError(de.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("questions/{page}")]
        public async Task<IActionResult> GetAllSurveyQuestions(int page = 1)
        {
            page = page <= 0 ? 1 : page;

            // Fetch paginated result
            IEnumerable<SurveyQuestion> allSurveyQuestions;
            try
            {
                allSurveyQuestions = await _surveyRepository.GetAllSurveyQuestionsAndSurveyOptionsPaginated(page, perPage);
                if (allSurveyQuestions.Count() == 0)
                    return NotFound(ResponseMessage.Message("Not found", errors: new { message = "No survey questions found" }));
            }
            catch (Exception de)
            {
                _logger.LogError(de.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }

            //  Reshape result
            var surveyQuestionList = new List<SurveyQuestionToReturnDTO>();
            foreach (var surveyQuestion in allSurveyQuestions)
            {
                var surveyOptionList = new List<QuestionOptionDTO>();
                foreach(var item in surveyQuestion.QuestionOptions)
                {
                    var result = new QuestionOptionDTO
                    {
                        Option = item.Option,
                        QuestionOptionId = item.QuestionOptionId
                    };

                    surveyOptionList.Add(result);
                }
                var surveyQuestionToReturn = new SurveyQuestionToReturnDTO
                {
                    SurveyQuestionId = surveyQuestion.SurveyQuestionId,
                    SurveyId = surveyQuestion.SurveyId,
                    Question = surveyQuestion.Question,
                    Options = surveyOptionList
                };

                surveyQuestionList.Add(surveyQuestionToReturn);
            }

            // new dto that contains pagination details 
            var pagedSurveyQuestions = new PaginatedSurveyQuestionsToReturnDTO
            {
                PageMetaData = Util.Paginate(page, perPage, _surveyRepository.TotalNumberOfItems),
                Data = surveyQuestionList
            };

            return Ok(ResponseMessage.Message("Survey questions found", data: pagedSurveyQuestions));
        }

        [Authorize(Roles = "admin, agent")]
        [HttpGet("question/{questionId}")]
        public async Task<IActionResult> GetSurveyQuestion(string questionId)
        {
            if (string.IsNullOrWhiteSpace(questionId))
                return BadRequest(ResponseMessage.Message("Invalid Id", errors: new { message = "Survey Type Id should not be null or empty or whitespace" }));

            // get question by id
            SurveyQuestion surveyQuestion;
            try
            {
                surveyQuestion = await _surveyRepository.GetSurveyQuestionById(questionId);
                if (surveyQuestion == null)
                    return NotFound(ResponseMessage.Message("Not found", errors: new { message = $"Survey question with id: {questionId} was not found" }));
            }
            catch (Exception de)
            {
                _logger.LogError(de.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }

            SurveyQuestionToReturnDTO surveyQuestionToReturn = null;
            try
            {
                var surveyOptionList = new List<QuestionOptionDTO>();
                foreach (var item in surveyQuestion.QuestionOptions)
                {
                    var result = new QuestionOptionDTO
                    {
                        Option = item.Option,
                        QuestionOptionId = item.QuestionOptionId
                    };

                    surveyOptionList.Add(result);
                }
                //shape DTO
                surveyQuestionToReturn = new SurveyQuestionToReturnDTO
                {
                    SurveyQuestionId = surveyQuestion.SurveyQuestionId,
                    SurveyId = surveyQuestion.SurveyId,
                    Question = surveyQuestion.Question,
                    Options = surveyOptionList
                };
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }

            return Ok(ResponseMessage.Message("Survey question found", data: surveyQuestionToReturn));
        }

        [HttpGet]
        [Route("survey-questions/{surveyId}")]
        [Authorize(Roles = "admin, agent")]
        public async Task<IActionResult> GetSurveyQuestionsBySurveyId(string surveyId)
        {
            if (string.IsNullOrWhiteSpace(surveyId))
                return BadRequest(ResponseMessage.Message("Empty survey Id", 
                    errors: new { message = "Survey Id must not be null or empty"}));

            try
            {
                var survey = await _surveyRepository.GetSurveyQuestionsBySurveyId(surveyId);
                if (survey == null)
                    return NotFound(ResponseMessage.Message("Null result",
                        errors: new { message = $"No record found for survey Id {surveyId}" }));

                return Ok(ResponseMessage.Message("Result found", 
                    data: survey ));
               
            }
            catch(NullReferenceException e)
            {
                return NotFound(ResponseMessage.Message("Null result",
                        errors: new { message = e.Message }));
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", 
                    errors: new { message = "Could not access record from data source, error written to log file" }));
            }
        }
        #endregion

        #region USER SURVEY ROUTE
        [Authorize(Roles = "admin")]
        [HttpPost("{surveyId}/assign-survey/{agentId}")]
        public async Task<IActionResult> AssignUserSurvey(string agentId, string surveyId)
        {
            if (String.IsNullOrWhiteSpace(agentId) || String.IsNullOrWhiteSpace(surveyId))
                return BadRequest(ResponseMessage.Message("Invalid credentials", errors: new { message = "Agent id or survey id should not be null or empty or whitespace" }));

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Could not access user" }));

            var userSurvey = new UserSurvey
            {
                ApplicationUserId = agentId,
                SurveyId = surveyId, 
                AddedBy = user.Id,
                UpdatedBy = user.Id
            };

            try
            {
                if(await _surveyRepository.GetUserSurveyByUserIdAndSurveyId(agentId, surveyId) != null)
                    return BadRequest(ResponseMessage.Message("BadRequest", errors: new { message = $"survey {surveyId} has already been assigned to this user" }));

                await _userSurveyRepository.Add(userSurvey);
                return Ok(ResponseMessage.Message("Success", data: new { userId = userSurvey.ApplicationUserId, surveyId = userSurvey.SurveyId }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = $"Failed to assign survey id = {surveyId} to agent id = {agentId}" }));
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("{surveyId}/reassign-agent/{agentId}")]
        public async Task<IActionResult> ReassignUserSurvey(string agentId, string surveyId, [FromBody] ReassignUserSurveyDTO model)
        {
            if (String.IsNullOrWhiteSpace(agentId) || String.IsNullOrWhiteSpace(surveyId))
                return BadRequest(ResponseMessage.Message("Invalid credentials", errors: new { message = "Agent id or survey id should not be null or empty or whitespace" }));

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Could not access user" }));

            UserSurvey userSurvey;
            try
            {
                userSurvey = await _surveyRepository.GetUserSurveyByUserIdAndSurveyId(agentId, surveyId);
                if (userSurvey == null)
                    return NotFound(ResponseMessage.Message("Not found", errors: new { message = "User Survey does not exist" }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }

            try
            {
                await _userSurveyRepository.Delete(userSurvey);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = $"Could not reassign new agent = {model.NewAgentId} to the survey id = {surveyId}" }));
            }            

            try
            {
                var newUserSurvey = new UserSurvey
                {
                    ApplicationUserId = model.NewAgentId,
                    Status = "pending",
                    SurveyId = surveyId,
                    AddedBy = user.Id,
                    UpdatedBy = user.Id
                };

                await _userSurveyRepository.Add(newUserSurvey);
                return Ok(ResponseMessage.Message("Success", data: new { message = $"Survey id {surveyId} reassigned successfully to user id {model.NewAgentId}" }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to reassign user survey" }));
            }
        }

        [Authorize(Roles = "agent")]
        [HttpPatch("acceptance-status")]
        public async Task<IActionResult>EditStatusUserSurvey([FromBody] UserSurveyDTO model) 
        {
            if (String.IsNullOrWhiteSpace(model.AgentId) || String.IsNullOrWhiteSpace(model.SurveyId))
                return BadRequest(ResponseMessage.Message("Invalid credentials", errors: new { message = "Agent id or survey id should not be null or empty or whitespace" }));

            if (model.Status != "accepted" && model.Status != "declined")
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Invalid status type given" }));

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Could not access user" }));

            UserSurvey userSurvey;
            try
            {
                userSurvey = await _surveyRepository.GetUserSurveyByUserIdAndSurveyId(model.AgentId, model.SurveyId);
                if(userSurvey == null)
                    return NotFound(ResponseMessage.Message("Not found", errors: new { message = "User Survey does not exist" }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }

            userSurvey.Status = model.Status;
            userSurvey.UpdatedBy = user.Id;

            try
            {
                await _userSurveyRepository.Update(userSurvey);
                return Ok(ResponseMessage.Message("Success", data: new { message = $"Status of survey id = {model.SurveyId} updated successfully" }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = $"Failed to update status of survey id = {model.SurveyId}" }));
            }
        }

        [Authorize(Roles = "agent")]
        [HttpPost("submit-survey")]
        public async Task<IActionResult> SubmitSurvey(ResponseUserSurveyDTO model)
        {
            if (String.IsNullOrWhiteSpace(model.AgentId) || String.IsNullOrWhiteSpace(model.SurveyId))
                return BadRequest(ResponseMessage.Message("Invalid credentials", errors: new { message = "Agent id or survey id should not be null or empty or whitespace" }));

            foreach(var item in model.Questions)
            {
                if(String.IsNullOrWhiteSpace(item.OptionId) || String.IsNullOrWhiteSpace(item.QuestionId))
                {
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Option id or question id should not be null or empty or whitespace" }));
                }
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Could not access user" }));

            UserSurvey userSurvey;
            try
            {
                userSurvey = await _surveyRepository.GetUserSurveyByUserIdAndSurveyId(model.AgentId, model.SurveyId);
                if (userSurvey == null)
                    return NotFound(ResponseMessage.Message("Not found", errors: new { message = "User Survey does not exist" }));
                if (userSurvey.Status != "accepted")
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "User did not accept this survey" }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }

            foreach (var item in model.Questions)
            {
                try
                {
                    // generate response id
                    string responseId;
                    Response result;
                    do
                    {
                        responseId = Guid.NewGuid().ToString();
                        result = await _surveyRepository.GetResponseByResponseId(responseId);
                    } while (result != null);

                    // construct response object
                    var newResponse = new Response
                    {
                        ResponseId = responseId,
                        ApplicationUserId = model.AgentId,
                        SurveyId = model.SurveyId,
                        SurveyQuestionId = item.QuestionId,
                        QuestionOptionId = item.OptionId
                    };

                    await _responseCrudRepo.Add(newResponse);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = $"Failed to add response of survey id = {model.SurveyId}" }));
                }            

            }            

            userSurvey.Status = "completed";

            try
            {
                await _userSurveyRepository.Update(userSurvey);
                return Ok(ResponseMessage.Message("Success", data: new { message = "User survey updated successfully" }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Bad request", errors: new { message = "Failed to update user survey to complete status" }));
            }
        }


        [HttpGet("{agentId}/{status}/{page}")]
        public async Task<IActionResult> GetUserSurveyForAgent(string agentId, string status, int page)
        {
            if (String.IsNullOrWhiteSpace(agentId) || String.IsNullOrWhiteSpace(status))
                return BadRequest(ResponseMessage.Message("Invalid credentials", errors: new { message = "Id or status should not be null or empty or whitespace" }));

            status = status.ToLower();
            if (status != "pending" && status != "accepted" && status != "declined" && status != "completed")
                return BadRequest(ResponseMessage.Message("Bad Request", errors: new { message = "Invalid status type given" }));

            page = page <= 0 ? 1 : page;
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(ResponseMessage.Message("Bad request", errors: new { message = "Could not access user" }));

            string Id;
            try
            {
                Id = _agentRepository.GetAgentById(agentId).Result.ApplicationUserId;
                if (Id == null)
                    return NotFound(ResponseMessage.Message("Not found", errors: new { message = "Agent does not exist" }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }

            try
            {
                var result = await _surveyRepository.GetUserSurveysByAgentIdandByStatusPaginated(Id, status, page, perPage);
                if (result.Count() == 0)
                    return NotFound(ResponseMessage.Message("Not found", errors: new { message = "No user surveys found" }));
                var totalUserSurvey = _surveyRepository.TotalNumberOfItems;

                var paginatedUserSurvey = result.Select(userSurvey => new UserSurveyToReturnDTO
                {
                    Topic = userSurvey.Survey.Topic,
                    Status = userSurvey.Status,
                    ApplicationUserId = userSurvey.Survey.AddedBy,
                    SurveyId = userSurvey.SurveyId
                })
                .ToList();

                var userSurveys = new PaginatedUserSurveyToReturnDTO
                {
                    PageMetaData = Util.Paginate(page, perPage, totalUserSurvey),
                    UserSurveyToReturn = paginatedUserSurvey
                };
                return Ok(ResponseMessage.Message("Success", data: userSurveys));
            }
            catch (Exception de)
            {
                _logger.LogError(de.Message);
                return BadRequest(ResponseMessage.Message("Data access error", errors: new { message = "Could not access record from data source, error written to log file" }));
            }
        }
        #endregion
    }
}
