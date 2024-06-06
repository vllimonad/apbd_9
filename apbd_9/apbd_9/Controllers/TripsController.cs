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
    
    [HttpGet]
    public IActionResult GetListOfTrips([FromQuery] int page = 1, [FromQuery] int size = 10)
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
            .Skip((page-1)*size)
            .Take(size)
            .ToListAsync();
        
        var totalPages = (int)Math.Ceiling((double)_context.Trips.Count()/size);

        var tripList = new TripListDTO
        {
            Page = page,
            PageSize = size,
            TotalPages = totalPages,
            Trips = method
        };
        return Ok(tripList);
    }
}