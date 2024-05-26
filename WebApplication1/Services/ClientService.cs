using Microsoft.EntityFrameworkCore;
using WebApplication1.Exceptions;
using WebApplication1.Models;

namespace WebApplication1.Services;

public class ClientService(Cw9Context dbContext) : IClientService
{
    public async Task RemoveClient(int idClient)
    {
        // Pobranie przypisanych wycieczek do klienta
        var assignedTrips = await dbContext.ClientTrips.Where(e => e.IdClient == idClient).ToListAsync();
        
        // Jezeli klient nie ma przypisanej zadnej wycieczki to rzucamy blad
        if (assignedTrips.Count != 0)
        {
            throw new BadRequestException($"Cannot remove client object with id:{idClient}, because it is assigned to one or more trips");
        }
        
        // Proba usuniecia klienta
        var affectedRows = await dbContext.Clients.Where(e => e.IdClient == idClient).ExecuteDeleteAsync();
        await dbContext.SaveChangesAsync(); 
        
        // Sprawdzenie czy klient zostal usuniety. Jezeli nie zostal, to zakladamy, ze juz nie iestnieje w bazie.
        if (affectedRows == 0)
        {
            throw new NotFoundException($"Client with id:{idClient} does not exist");
        }
    }
}