namespace HealthCareABApi.DTO
{
    public class AvailabilityDTO
    {
        public AvailabilityDTO(List<DateTime> availableSlots)
        {
            AvailableSlots = availableSlots;
        }

        private List<DateTime> AvailableSlots { get; set; }
    }
}
