using System;

namespace Server.Objects.Domain
{
    public class Review
    {
        public Guid Id { get; set; }
        public double Punctuation { get; set; }
        public string Comment { get; set; }

        public Review(double punctuation, string comment)
        {
            Id = Guid.NewGuid();
            Punctuation = punctuation;
            Comment = comment;
        }
    }
}