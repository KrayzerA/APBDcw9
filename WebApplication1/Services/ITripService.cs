using WebApplication1.RequestModel;
using WebApplication1.RequestModels;

namespace WebApplication1.Services;

public interface ITripService
{
    Task<List<GetTripsResponseModel>> GetTripsAsync(CancellationToken cancellationToken);

    Task AssignAClientToTheTripAsync(int idTrip, AssignAClientToTheTripRequestModel requestModel,
        CancellationToken cancellationToken);
}