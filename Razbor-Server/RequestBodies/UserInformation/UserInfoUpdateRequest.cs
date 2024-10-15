using System.ComponentModel.DataAnnotations;

namespace RequestBodies.UserInformation
{
    public class UserInfoUpdateRequest
    {
        [EmailAddress]
        public string? Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? Birthday { get; set; }

        public double? Height { get; set; }

        public double? Weight { get; set; }

        public string? SelectedSong { get; set; }

        public string? Description { get; set; }

        [Url]
        public string? PrimaryPicture { get; set; }
    }
}
