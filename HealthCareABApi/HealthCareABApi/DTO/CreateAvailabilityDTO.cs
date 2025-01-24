namespace HealthCareABApi.DTO
{
    public class CreateAvailabilityDTO
    {
        public CreateAvailabilityDTO(string caregiverId, DateTime dateTime)
        {
            CaregiverId = caregiverId;
            DateTime = dateTime;
        }

        public string CaregiverId { get; private set; }

        public DateTime DateTime { get; private set; } = new DateTime(1111, 11, 11); // Initialize as an invalid date
    }
}

