using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasker.API.Controllers.Base;
using Tasker.API.Exceptions;
using Tasker.API.Models.Transfer.Team;
using Tasker.API.Services;

namespace Tasker.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class TeamsController : ApiControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService, IMapper mapper, IUserService userService, IConfiguration configuration) : base(mapper, userService, configuration)
        {
            _teamService = teamService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TeamInfoModel>>> GetTeams()
        {
            var teams = await _teamService.GetAllTeamsAsync();

            List<TeamInfoModel> infoModels = _mapper.Map<List<TeamInfoModel>>(teams);

            return Ok(infoModels);
        }

        [HttpGet("{teamId}")]
        public async Task<ActionResult<TeamInfoModel>> GetTeamById(string teamId)
        {
            var team = await _teamService.GetByIdAsync(teamId);

            TeamInfoModel infoModel = _mapper.Map<TeamInfoModel>(team);

            return Ok(infoModel);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult<TeamInfoModel>> CreateTeam(string name)
        {
            var user = GetLoggedInUser();

            if (user == null)
                return Unauthorized();

            try
            {
                await _teamService.CreateAsync(user, name);

                return Created();
            }
            catch (DuplicateEntityException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{teamId}/rename")]
        public async Task<ActionResult<TeamInfoModel>> RenameTeam(string teamId, string name)
        {
            var user = GetLoggedInUser();

            var team = await _teamService.GetByIdAsync(teamId);
            if (team == null)
                return NotFound();

            if (team.LeaderId != user.Id)
                return Forbid();

            try
            {
                var updatedTeam = await _teamService.UpdateAsync(teamId, name);
                return Ok(_mapper.Map<TeamInfoModel>(updatedTeam));
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("{teamId}/members")]
        public async Task<ActionResult> AddUserToTeam(string teamId, string userId)
        {
            var user = GetLoggedInUser();

            var team = await _teamService.GetByIdAsync(teamId);
            if (team == null)
                return NotFound();

            if (team.LeaderId != user.Id)
                return Forbid();

            try
            {
                await _teamService.AddUserToTeamAsync(teamId, userId);

                return Ok();
            }
            catch (EntityNotFoundException ex)
            {
                if (ex.EntityName == "User")
                {
                    return BadRequest("User could not be found!");
                }
                else
                {
                    return BadRequest("Team could not be found!");
                }
            }
            catch (DuplicateEntityException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{teamId}/members")]
        public async Task<ActionResult> RemoveUserFromTeam(string teamId, string userId)
        {
            var user = GetLoggedInUser();

            var team = await _teamService.GetByIdAsync(teamId);
            if (team == null)
                return NotFound();

            if (team.LeaderId != user.Id)
                return Forbid();

            try
            {
                await _teamService.RemoveUserFromTeamAsync(teamId, userId);

                return Ok();
            }
            catch (InvalidDomainOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{teamId}/delete")]
        public async Task<ActionResult> DeleteTeam(string teamId)
        {
            try
            {
                await _teamService.DeleteTeamAsync(teamId);

                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("{teamId}/leader")]
        public async Task<ActionResult<TeamInfoModel>> UpdateTeamLeader(string teamId, string newLeaderId)
        {
            var user = GetLoggedInUser();

            var team = await _teamService.GetByIdAsync(teamId);
            if (team == null)
                return NotFound();

            if (team.LeaderId != user.Id)
                return Forbid();

            try
            {
                var result = await _teamService.UpdateTeamLeaderAsync(teamId, newLeaderId);

                TeamInfoModel infoModel = _mapper.Map<TeamInfoModel>(result);

                return Ok(infoModel);
            }
            catch (EntityNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidDomainOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
