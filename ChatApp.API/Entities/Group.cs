using System.ComponentModel.DataAnnotations;

namespace ChatApp.API.Entities;

public class Group
{
    [Key]
    public string Name { get; set; }
    public ICollection<Connection> Connections { get; set; } = [];
}
