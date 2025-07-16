using System.ComponentModel.DataAnnotations;

namespace Tasker.API.Models.Database
{
    public class TeamUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public required string UserId { get; set; }
        public TaskerUser User { get; set; } = null!;

        public required string TeamId { get; set; }
        public Team Team { get; set; } = null!;

        // Use a string or custom enum
        [MaxLength(50)]
        public TeamRole Role { get; set; } = TeamRole.Member;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
