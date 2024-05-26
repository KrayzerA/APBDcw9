using FluentValidation;
using WebApplication1.Exceptions;
using WebApplication1.RequestModels;
using WebApplication1.Services;

namespace WebApplication1.Endpoints;

public static class TripsEndpoints
{
    public static void RegisterTripsEndpoints(this RouteGroupBuilder builder)
    {
        // Utworzenie grupy dla koncowek wycieczek
        var group = builder.MapGroup("trips");

        // Zmapowanie poszczegolnych metod do grupy
        group.MapGet("", GetTrips);
        group.MapPost("{idTrip:int}/clients", AssignAClientToTheTrip);
    }

    private static async Task<IResult> GetTrips(ITripService service, CancellationToken cancellationToken)
    {
        /*
         * Zwracamy odpowiedni obiekt response model z serwisu.
         */
        return Results.Ok(await service.GetTripsAsync(cancellationToken));
    }

    private static async Task<IResult> AssignAClientToTheTrip(
        int idTrip,
        AssignAClientToTheTripRequestModel requestModel, 
        IValidator<AssignAClientToTheTripRequestModel> validator,
        ITripService service, CancellationToken cancellationToken)
    {
        /*
         * Walidacja ciala zadania, przed przekazaniem danych do serwisu (FluentValidation).
         * Jezeli dane nie spelniaja zalozen, to zwracamy blad.
         */
        var validate = await validator.ValidateAsync(requestModel, cancellationToken);
        if (!validate.IsValid)
        {
            return Results.BadRequest(validate.ToDictionary());
        }

        /*
         * Przypisujemy podanego klienta do wycieczki. J
         *  - Jezeli klient nie istnieje, to dodajemy go do bazy danych.
         *  - Jezeli klient istnieje, to sprawdzamy, czy nie jest juz przypisany do takiej wycieczki. Jezeli jest,
         *    to zwracamy stosowny blad.
         * Nastepnie sprawdzamy, czy wycieczka istnieje. Jezeli nie istnieje to zwracamy stosowny blad.
         * Jako ostatnie tworzymy nowe przypisanie klienta do wycieczki i zwracamy odpowiedni kod http.
         */
        try
        {
            await service.AssignAClientToTheTripAsync(idTrip, requestModel, cancellationToken);
            return Results.NoContent();
        }
        catch (NotFoundException e)
        {
            // Wycieczka nie istnieje
            return Results.NotFound(e.Message);
        }
        catch (BadRequestException e)
        {
            // Klient jest juz przypisany do wycieczki
            return Results.BadRequest(e.Message);
        }
    }
}