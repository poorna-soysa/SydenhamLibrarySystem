namespace SydenhamLibrarySystem.Models.DTOs;

public class ReservationDetailModalDTO
{
    public string DivId { get; set; }
    public IEnumerable<ReservationDetail> ReservationDetail { get; set; }
}
