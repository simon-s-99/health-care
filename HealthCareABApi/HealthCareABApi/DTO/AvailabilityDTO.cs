namespace HealthCareABApi.DTO
{
    public class AvailabilityDTO
    {
        public AvailabilityDTO(DateTime dateTime)
        {
            DateTime = dateTime;
        }

        private DateTime DateTime { get; set; } = new DateTime(1111, 11, 11); // Initialize as an invalid date
    }
}
