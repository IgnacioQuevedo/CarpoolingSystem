using System;

namespace MainServer.Objects.DTOs.ReviewModelDtos
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public double Punctuation { get; set; }
        public string Comment { get; set; }

        public ReviewDto(Guid id ,double punctuation,string comment)
        {
            Id = id;
            Punctuation = punctuation;
            Comment = comment;
        }
    }
    
}