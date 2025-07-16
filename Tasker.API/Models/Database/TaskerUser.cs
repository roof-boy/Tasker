using Microsoft.AspNetCore.Identity;

namespace Tasker.API.Models.Database
{
    public class TaskerUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool? Activated { get; set; } = false;
        public ICollection<TeamUser> Teams { get; set; } = new List<TeamUser>();
    }
}
