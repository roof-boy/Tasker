using System.ComponentModel.DataAnnotations;

namespace Tasker.API.Models.Database
{
    public class Team
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Creator of the team (usually same as Leader at creation)
        public required string CreatedById { get; set; }
        public TaskerUser CreatedBy { get; set; } = null!;

        // Current leader or admin of the team
        public string? LeaderId { get; set; }
        public TaskerUser? Leader { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<TeamUser> TeamUsers { get; set; } = new List<TeamUser>();
    }
}