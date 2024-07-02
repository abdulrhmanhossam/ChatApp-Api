namespace ChatApp.API.Entities;
public class Message
{
    public int Id { get; set; }
    public string SenderUsername { get; set; }
    public string RecipientUsername { get; set; }
    public string Content { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime MessageSent { get; set; } = DateTime.UtcNow;
    public bool SenderDeleted { get; set; }
    public bool RecipientDeleted { get; set; }

    // Nav Prop
    public int SenderId { get; set; }
    public AppUser Sender { get; set; } = null!;
    public int RecipientId { get; set; }
    public AppUser Recipient { get; set; } = null!;
}
