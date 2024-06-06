namespace apbd_9.DTOs;

public class TripDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public IEnumerable<ClientDTO> Countries { get; set; }
    public int MaxPeople { get; set; }
    public IEnumerable<ClientDTO> Clients { get; set; }
}