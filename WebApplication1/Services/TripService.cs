using Microsoft.EntityFrameworkCore;
using WebApplication1.Exceptions;
using WebApplication1.Models;
using WebApplication1.RequestModel;
using WebApplication1.RequestModels;

namespace WebApplication1.Services;

public class TripService(Cw9Context context) : ITripService
{
    public async Task<List<GetTripsResponseModel>> GetTripsAsync(CancellationToken cancellationToken)
    {
        // Pobranie i zmapowanie rekordow z bazy danych na dto
        return await context.Trips.Select(trip => new GetTripsResponseModel
            {
                Name = trip.Name,
                Description = trip.Description,
                DateFrom = trip.DateFrom,
                DateTo = trip.DateTo,
                MaxPeople = trip.MaxPeople,
                Countries = trip.IdCountries.Select(country => new CountryDetails
                {
                    Name = country.Name
                }).ToList(),
                Clients = trip.ClientTrips.Select(clientTrip => new ClientDetails
                {
                    FirstName = clientTrip.IdClientNavigation.FirstName,
                    LastName = clientTrip.IdClientNavigation.LastName
                }).ToList()
            }).OrderByDescending(e => e.DateFrom)
            .ToListAsync(cancellationToken);
    }

    public async Task AssignAClientToTheTripAsync(int idTrip, AssignAClientToTheTripRequestModel requestModel, CancellationToken cancellationToken)
    {
        // Proba pobrania klienta o danym peselu
        var client = await context.Clients.SingleOrDefaultAsync(e => e.Pesel == requestModel.Pesel, cancellationToken);
        
        // Jezeli klent nie istnieje, to nalezy go utworzyc i dodac do bazy
        if (client is null)
        {
            client = new Client
            {
                FirstName = requestModel.FirstName,
                LastName = requestModel.LastName,
                Email = requestModel.Email,
                Telephone = requestModel.Telephone,
                Pesel = requestModel.Pesel
            };
           await context.AddAsync(client, cancellationToken);
        }
        // Jezeli klient istnieje, to sprawdzamy, czy nie jest juz on przypisany do tej wycieczki
        else
        {
            var tripAssignment =
                await context.ClientTrips.SingleOrDefaultAsync(e =>
                    e.IdClient == client.IdClient && e.IdTrip == idTrip, cancellationToken);
            // Jezeli obiekt przypisania istnieje, to zwracamy stosowny blad
            if (tripAssignment is not null)
            {
                throw new BadRequestException("Client is assigned to this trip already");
            }
        }

        // Pobranie wycieczki o danym id
        var trip = await context.Trips.SingleOrDefaultAsync(e => e.IdTrip == idTrip, cancellationToken);
        // Jezeli wycieczka nie istnieje, to zwracamy stosowny blad
        if (trip is null)
        {
            throw new NotFoundException($"Trip with id:{idTrip} does not exist");
        }

        // Utworzenie obiektu przypisania i dodanie go do bazy danych
        var clientTrip = new ClientTrip
        {
            /*
             * Jako ze id klienta moze w tym momencie jeszcze nie istniec (nie zostala zatwierdzona transakcja,
             * wiec moze byc jeszcze niewygenerowane, to zamiast id przypisujemy referencje do obiektu.
             * Entityframework z autoamtu przypisze id w bazie danych do tego rekordu,
             * jak tylko wykona operacje dodania klienta do bazy danych.
             *
             * Przypisanie referencji odbywa sie poprzez pole nawigacyjne w klasie,
             * ktore sluzy do zaimplementowania relacji bazodanowych w sposob obiektowy
             * (patrz pole IdClientNacigation w modelu ClientTrip).
             */
            IdClientNavigation = client,
            /*
             * Taka sama sytuacja, co wyzej.
             * (patrz pole IdTripNavigation w modelu ClientTrip)
             */
            IdTripNavigation = trip,
            PaymentDate = requestModel.PaymentDate,
            RegisteredAt = DateTime.Now
        };
        await context.ClientTrips.AddAsync(clientTrip, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}