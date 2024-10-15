using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Tables
{
    [Table("ChatTable")]
    public class ChatTable
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string MatchId { get; set; }

        [Required]
        public string Sender { get; set; } = null!;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Message { get; set; } = null!;

        public MatchTable Match { get; set; }
    }
}
