using apbd_9.Models;

namespace apbd_9.DTOs;

public class TripListDTO
{
    public int pageNum { get; set; }
    public int pageSize { get; set; }
    public int allPages { get; set; }
    public Task<List<TripDTO>> trips { get; set; }
}

public class TripDTO
{
    public string Name { get; set; }
    public DateTime DateFrom { get; set; }
    public int MaxPeople { get; set; }
    public IEnumerable<Client> Clients { get; set; }
}