using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Core.Models;
public class Role{
    public Guid Id {get;set;}
    [Required]
    public string Name {get;set;} = null!;
    [JsonIgnore]
    public ICollection<User>? users {get;set;} = new List<User>();
}