using System.ComponentModel.DataAnnotations; //We need to add using System.ComponentModel.DataAnnotations to use the Required attribute

namespace CreekRiver.Models;

public class CampsiteType
{
    public int Id { get; set; }
    [Required]
    public string CampsiteTypeName { get; set; }
    public int MaxReservationDays { get; set; }
    public decimal FeePerNight { get; set; }
}

//Line 8 Required is what EFCore looks at and knows to make sure a field is not left blank or null....it knows to make that entry into the database NOT NULL