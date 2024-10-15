using System.ComponentModel.DataAnnotations;

namespace RequestBodies.Chat
{
    public class SendMessageRequest
    {
        [Required]
        public string Recipient { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
