using System.ComponentModel.DataAnnotations;
using Database.Enums;

namespace RequestBodies.Authentication
{
    public class RegisterRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public Gender UserGender { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

        public double Height { get; set; }

        public double Weight { get; set; }

        public string SelectedSong { get; set; }

        public string Description { get; set; }

        public string PrimaryPicture { get; set; }
    }
}
