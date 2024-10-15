using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Tables
{
    [Table("MatchTable")]
    public class MatchTable
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required]
        public string OriginUser { get; set; } = null!;

        public UserInfoTable OriginUserTable { get; set; } = null!;

        [Required]
        public bool OriginMatchStatus { get; set; }

        [Required]
        public string DestinationUser { get; set; } = null!;

        public UserInfoTable DestinationUserTable { get; set; } = null!;

        [Required]
        public bool DestinationMatchStatus { get; set; }

        public ICollection<ChatTable> Chat { get; set; } = new List<ChatTable>();
    }
}
