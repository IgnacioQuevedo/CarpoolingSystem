using System;

namespace Client.Objects.ReviewModels
{
    public class ReviewClient
    {
        public Guid Id { get; set; }
        public double Punctuation { get; set; }
        public string Comment { get; set; }

        public ReviewClient(Guid id, double punctuation, string comment)
        {
            Id = id;
            Punctuation = punctuation;
            Comment = comment;
        }
    }
}