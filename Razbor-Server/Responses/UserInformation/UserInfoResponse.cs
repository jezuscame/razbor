using Database.Enums;

namespace Responses.UserInformation
{
    public class UserInfoResponse
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime Birthday { get; set; }

        public Gender UserGender { get; set; }

        public double? Height { get; set; }

        public double? Weight { get; set; }

        public string? SelectedSong { get; set; }

        public string? Description { get; set; }

        public string? PrimaryPicture { get; set; }
    }
}
