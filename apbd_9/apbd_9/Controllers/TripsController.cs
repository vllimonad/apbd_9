using apbd_9.Data;
using apbd_9.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apbd_9.Controllers;

[Route("api")]
[ApiController]
public class TripsController : ControllerBase
{
    private readonly Task9Context _context;
    
    public TripsController(Task9Context context)
    {
        _context = context;
    }
    
    [HttpGet("/trips")]
    public IActionResult GetListOfTrips([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var method = _context.Trips.Select(e => new TripDTO()
            {
                Name = e.Name,
                DateFrom = e.DateFrom,
                MaxPeople = e.MaxPeople,
                Clients = e.ClientTrips.Select(e => new ClientDTO()
                {
                    FirstName = e.IdClientNavigation.FirstName,
                    LastName = e.IdClientNavigation.LastName
                })
            })
            .OrderBy(e => e.DateFrom)
            .Skip((pageNumber-1)*pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        var totalPages = (int)Math.Ceiling((double)_context.Trips.Count()/pageSize);

        var tripList = new TripListDTO
        {
            pageNum = pageNumber,
            pageSize = pageSize,
            allPages = totalPages,
            trips = method
        };
        return Ok(tripList);
    }

    [HttpDelete("/clients/{idClient}")]
    public IActionResult delteClient(int idClient)
    {
        var client = _context.Clients.Find(idClient);
        if (client == null)
        {
            return NotFound("This client does not exist");
        }

        var hasTrip = _context.ClientTrips.Any(c => c.IdClient == idClient);
        if (hasTrip)
        {
            return BadRequest("This client has at least 1 trip");
        }

        _context.Clients.Remove(client);
        _context.SaveChanges();
        return Ok("Client was deleted");
    } 
}