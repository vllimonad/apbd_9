namespace apbd_9.DTOs;

public class TripListDTO
{
    public int pageNum { get; set; }
    public int pageSize { get; set; }
    public int allPages { get; set; }
    public Task<List<TripDTO>> trips { get; set; }
}