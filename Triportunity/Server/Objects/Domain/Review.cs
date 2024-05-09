using Server.Exceptions;
using System;

namespace Server.Objects.Domain
{
    public class Review
    {
        private double _punctuation;

        public Guid Id { get; private set; }

        public double Punctuation
        {
            get => _punctuation;
            set
            {
                if (value < 0.0 || value > 5.0)
                {
                    throw new ReviewException("The punctuation must be between 0.0 and 5.0.");
                }
                _punctuation = value;
            }
        }

        private string _comment;

        public string Comment
        {
            get => _comment;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ReviewException("The comment cannot be empty.");
                }
                _comment = value;
            }
        }

        public Review(double punctuation, string comment)
        {
            Id = Guid.NewGuid();
            Punctuation = punctuation; 
            Comment = comment; 
        }
    }
}
