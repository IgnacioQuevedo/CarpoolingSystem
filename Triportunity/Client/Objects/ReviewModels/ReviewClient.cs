using System;

namespace Client.Objects.ReviewModels
{
    public class ReviewClient
    {
        public Guid DriverId { get; set; }
        public double Punctuation { get; set; }
        public string Comment { get; set; }

        public ReviewClient()
        {

        }
        public ReviewClient(Guid driverId, double punctuation, string comment)
        {
            Punctuation = punctuation;
            Comment = comment;
            DriverId = driverId;
        }
    }
}