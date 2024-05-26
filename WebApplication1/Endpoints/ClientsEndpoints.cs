using Microsoft.AspNetCore.Http.HttpResults;
using WebApplication1.Exceptions;
using WebApplication1.Services;

namespace WebApplication1.Endpoints;

public static class ClientsEndpoints
{
    public static void RegisterClientsEndpoints(this RouteGroupBuilder build)
    {
        // Utworzenie grupy dla koncowek klientow
        var group = build.MapGroup("clients");
        
        // Zmapowanie poszczegolnych metod do grupy
        group.MapDelete("{idClient:int}", RemoveClient);
    }

    private static async Task<IResult> RemoveClient(int idClient, IClientService service)
    {
        /*
         * Usun klienta o danym id z bazy. Jezeli klient jest przypisany do jakiejs wycieczki,
         * to nie mozemy go usunac i musi zostac zworcony blad. Jezeli klient o danym id nie istnieje,
         * to rowniez zwracamy stosowny blad.
         *
         * Jezeli klienta udalo sie usunac, to zwracamy stosowny pozytywny kod http.
         */
        try
        {
            await service.RemoveClient(idClient);
        }
        catch (NotFoundException exception)
        {
            // Klient nie istnieje
            return Results.NotFound(exception.Message);
        }
        catch (BadRequestException exception)
        {
            // Klient ma przypisane wycieczki do siebie i nie mozna go usunac
            return Results.BadRequest(exception.Message);
        }

        return Results.NoContent();
    }
}