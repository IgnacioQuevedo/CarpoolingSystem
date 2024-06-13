using Grpc.Net.Client;
using Trip;

namespace AdministrativeServer.Services
{
    public class AdministrativeClient
    {
        private readonly AdministrativeService.AdministrativeServiceClient _client;

        public AdministrativeClient(string address)
        {
            var channel = GrpcChannel.ForAddress(address);
            _client = new AdministrativeService.AdministrativeServiceClient(channel);
        }

        public async Task<TripResponse> AddTripAsync(TripRequest request)
        {
            return await _client.AddTripAsync(request);
        }

        public async Task<TripResponse> DeleteTripAsync(TripRequest request)
        {
            return await _client.DeleteTripAsync(request);
        }

        public async Task<TripResponse> UpdateTripAsync(TripRequest request)
        {
            return await _client.UpdateTripAsync(request);
        }

        public async Task<TripRatingsResponse> GetTripRatingsAsync(TripRequest request)
        {
            return await _client.GetTripRatingsAsync(request);
        }

        public async Task<TripsResponse> GetNextTripsAsync(TripsRequest request)
        {
            return await _client.GetNextTripsAsync(request);
        }
    }
