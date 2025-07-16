using System.ComponentModel.DataAnnotations;
using Tasker.API.Models.Database;
using Tasker.API.Models.Transfer.User;

namespace Tasker.API.Models.Transfer.Team
{
    public class TeamInfoModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public UserInfoModel CreatedBy { get; set; } = null!;
        public UserInfoModel Leader { get; set; } = null!;

        // Possible use later on
        // public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Also possible use later on
        // public ICollection<TeamUser> TeamUsers { get; set; } = new List<TeamUser>();
    }
}
