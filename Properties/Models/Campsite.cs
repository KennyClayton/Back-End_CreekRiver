using System.ComponentModel.DataAnnotations; //We need to add using System.ComponentModel.DataAnnotations to use the Required attribute

namespace CreekRiver.Models;

public class Campsite
{
    public int Id { get; set; }
    [Required]
    public string Nickname { get; set; }
    public string ImageUrl { get; set; }
    public int CampsiteTypeId { get; set; }
    public CampsiteType CampsiteType { get; set; }
    public List<Reservation> Reservations { get; set; }
}