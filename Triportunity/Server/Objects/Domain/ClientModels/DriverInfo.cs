using System.Collections.Generic;
using Server.Exceptions;

namespace Server.Objects.Domain.ClientModels
{
    public class DriverInfo
    {
        public string Ci { get; set; }
        public double Puntuation { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }

        public DriverInfo(string ci,ICollection<Vehicle> driverVehicles)
        {
            Ci = ci;
            Puntuation = 5.0;
            Reviews = new List<Review>();
            Vehicles = driverVehicles;
            
            DriverInfoValidations();
        }

        private void DriverInfoValidations()
        {
            CheckIfCiIsEmpty();
            ValidateThatExistsVehicles();
        }

        private void ValidateThatExistsVehicles()
        {
            if (Vehicles.Count == 0)
            {
                throw new DriverInfoException("At least one vehicle must be declared");
            }
        }

        private void CheckIfCiIsEmpty()
        {
            int minimalLengthForCi = 6;
            
            if (string.IsNullOrEmpty(Ci))
            {
                throw new DriverInfoException("Ci must be declared.");
            }

            if (Ci.Length < minimalLengthForCi || !NumericFormatIsCorrect())
            {
                throw new DriverInfoException("Ci must be in a correct format. It must be at least of" +
                                              minimalLengthForCi + "and without special characters");
            }
        }
        private bool NumericFormatIsCorrect()
        {
            foreach (char c in Ci)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }
    }
}