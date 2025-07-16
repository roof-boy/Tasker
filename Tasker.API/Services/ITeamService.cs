using Microsoft.EntityFrameworkCore;
using Tasker.API.Data;
using Tasker.API.Models.Database;
using Tasker.API.Exceptions;
using Tasker.API.Models;

namespace Tasker.API.Services
{
    public interface ITeamService
    {
        Task<List<Team>> GetAllTeamsAsync();

        Task<Team?> GetByIdAsync(string id);

        Task<List<Team>> GetTeamsForUserAsync(string userId);

        Task<Team> CreateAsync(TaskerUser creatingUser, string name);

        Task<Team> UpdateAsync(string teamId, string newName);

        Task<TeamUser> AddUserToTeamAsync(string teamId, string userId);

        Task RemoveUserFromTeamAsync(string teamId, string userId);

        Task<Team> UpdateTeamLeaderAsync(string teamId, string newLeaderUserId);

        Task DeleteTeamAsync(string teamId);
    }

    public class TeamService : ITeamService
    {
        private readonly TaskerDbContext _context;
        private readonly IUserService _userService;

        public TeamService(TaskerDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<TeamUser> AddUserToTeamAsync(string teamId, string userId)
        {
            var user = await _userService.GetByIdAsync(userId);
            if (user is null)
                throw new EntityNotFoundException("User", userId);

            var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == teamId);
            if (team is null)
                throw new EntityNotFoundException("Team", userId);

            var existingTeamUser = await _context.TeamUsers
                .AnyAsync(tu => tu.TeamId == teamId && tu.UserId == userId);
            if (existingTeamUser)
                throw new DuplicateEntityException("User", $"User '{userId}' is already a member of Team '{teamId}'.");

            var teamUser = new TeamUser
            {
                TeamId = teamId,
                UserId = userId,
                User = user,
                Team = team,
                Role = TeamRole.Member,
                JoinedAt = DateTime.UtcNow
            };

            await _context.TeamUsers.AddAsync(teamUser);
            await _context.SaveChangesAsync();

            return teamUser;
        }


        public async Task<Team> CreateAsync(TaskerUser creatingUser, string name)
        {
            var existingTeam = await _context.Teams.FirstOrDefaultAsync(t => t.Name == name);
            if (existingTeam != null)
                throw new DuplicateEntityException("Team", $"Team with name '{name}' already exists.");

            var team = new Team
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                CreatedById = creatingUser.Id,
                LeaderId = creatingUser.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Teams.Add(team);

            var teamUser = new TeamUser
            {
                TeamId = team.Id,
                UserId = creatingUser.Id,
                User = creatingUser,
                Team = team,
                Role = TeamRole.Leader,
                JoinedAt = DateTime.UtcNow
            };

            _context.TeamUsers.Add(teamUser);

            await _context.SaveChangesAsync();

            var returnedTeam = await _context.Teams
                .Include(t => t.Leader)
                .Include(t => t.CreatedBy)
                .FirstAsync(t => t.Id == team.Id);

            return returnedTeam;
        }


        public async Task DeleteTeamAsync(string teamId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == teamId);
            if (team == null)
                throw new EntityNotFoundException("Team", teamId);

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Team>> GetAllTeamsAsync()
        {
            return await _context.Teams.Include(t => t.Leader).Include(t => t.CreatedBy).ToListAsync();
        }

        public async Task<Team?> GetByIdAsync(string id)
        {
            return await _context.Teams.Include(t => t.Leader).Include(t => t.CreatedBy).FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Team>> GetTeamsForUserAsync(string userId)
        {
            var teams = await _context.Teams
                .Where(t => _context.TeamUsers
                    .Where(tu => tu.UserId == userId)
                    .Select(tu => tu.TeamId)
                    .Contains(t.Id))
                .Include(t => t.Leader)
                .Include(t => t.CreatedBy)
                .ToListAsync();

            return teams;
        }

        public async Task RemoveUserFromTeamAsync(string teamId, string userId)
        {
            var teamUser = await _context.TeamUsers.SingleOrDefaultAsync(tu => tu.TeamId == teamId && tu.UserId == userId);

            if (teamUser == null)
                throw new InvalidDomainOperationException($"User {userId} is not part of Team {teamId}");

            _context.TeamUsers.Remove(teamUser);
            await _context.SaveChangesAsync();
        }

        public async Task<Team> UpdateAsync(string teamId, string newName)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == teamId);
            if (team == null)
                throw new EntityNotFoundException("Team", teamId);

            team.Name = newName;

            _context.Teams.Update(team);
            await _context.SaveChangesAsync();

            var returnedTeam = await _context.Teams
                .Include(t => t.TeamUsers)
                .Include(t => t.Leader)
                .Include(t => t.CreatedBy)
                .FirstAsync(t => t.Id == team.Id);

            return returnedTeam;
        }

        public async Task<Team> UpdateTeamLeaderAsync(string teamId, string newLeaderUserId)
        {
            var newLeader = await _userService.GetByIdAsync(newLeaderUserId);
            if (newLeader == null)
                throw new EntityNotFoundException("User", newLeaderUserId);

            var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == teamId);
            if (team == null)
                throw new EntityNotFoundException("Team", teamId);

            var userInTeam = await _context.TeamUsers.SingleOrDefaultAsync(tu => tu.TeamId == teamId && tu.UserId == newLeaderUserId);
            if (userInTeam != null)
                throw new InvalidDomainOperationException($"User {newLeaderUserId} does not belong to Team {teamId}");

            team.Leader = newLeader;
            team.LeaderId = newLeader.Id;

            _context.Teams.Update(team);
            await _context.SaveChangesAsync();

            await _context.SaveChangesAsync();

            var returnedTeam = await _context.Teams
                .Include(t => t.TeamUsers)
                .Include(t => t.Leader)
                .Include(t => t.CreatedBy)
                .FirstAsync(t => t.Id == team.Id);

            return returnedTeam;
        }
    }
}
