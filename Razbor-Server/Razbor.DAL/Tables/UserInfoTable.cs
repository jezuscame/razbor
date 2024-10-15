using Database.Enums;
using Database.Tables;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("UserInfoTable")]
public class UserInfoTable : IEquatable<UserInfoTable>
{
    public string Username { get; set; }

    public string Password { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public Gender UserGender { get; set; }

    public DateTime Birthday { get; set; }

    public double Height { get; set; }

    public double Weight { get; set; }

    public string SelectedSong { get; set; }

    public string Description { get; set; }

    [Url]
    public string PrimaryPicture { get; set; }

    public IList<MatchTable> DestinationMatches { get; set; } = new List<MatchTable>();

    public IList<MatchTable> OriginMatches { get; set; } = new List<MatchTable>();

    public bool Equals(UserInfoTable? other) =>
        Username == other?.Username;
}