namespace apbd_9.DTOs;

public class TripListDTO
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public Task<List<TripDTO>> Trips { get; set; }
}