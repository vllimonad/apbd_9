using apbd_9.Data;
using apbd_9.DTOs;
using apbd_9.Models;
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
                Clients = e.ClientTrips.Select(e => new Client()
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

    [HttpPost("trips/{idTrip}/clients")]
    public IActionResult assignClient(int idTrip, ClientDTO clientDto)
    {
        var DoesPeselExist = _context.Clients.Any(c => c.Pesel.Equals(clientDto.Pesel));
        if (DoesPeselExist)
        {
            return BadRequest("Client with this PESEL already exist");
        }

        var clientsIDs = _context.ClientTrips.Where(c => c.IdTrip == idTrip).Select(c => c.IdClient);
        var isRegistered = clientsIDs.Any(id => _context.Clients.Find(id).Pesel.Equals(clientDto.Pesel));
        if (isRegistered)
        {
            return BadRequest("Client with this PESEL already registered for this trip");
        }

        var trip = _context.Trips.Find(idTrip);
        if (trip == null)
        {
            return BadRequest("This trip does not exist");
        }
        
        if (trip.DateFrom <= DateTime.Now)
        {
            return BadRequest("This trip is already occurred");
        }

        var client = new Client
        {
            FirstName = clientDto.FirstName,
            LastName = clientDto.LastName,
            Email = clientDto.Email,
            Telephone = clientDto.Telephone,
            Pesel = clientDto.Pesel
        };
        _context.Clients.Add(client);
        _context.SaveChanges();
            
        _context.ClientTrips.Add(new ClientTrip
        {
            IdClient = client.IdClient,
            IdTrip = idTrip,
            PaymentDate = clientDto.PaymentDate,
            RegisteredAt = DateTime.Now
        });
        _context.SaveChanges();
        return Ok();
    }
}