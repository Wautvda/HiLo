using System.ComponentModel.DataAnnotations;

namespace HiLo.Domain;

public class Player
{
    [MaxLength(50)]
    public required string Name { get; set; }
}