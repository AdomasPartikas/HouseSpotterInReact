using System.ComponentModel.DataAnnotations.Schema;

namespace HouseSpotter.Server.Models
{
    public class User
    {
        public int ID { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<string>? SavedSearches { get; set; }
        public bool IsAdmin { get; set; }
    }
}