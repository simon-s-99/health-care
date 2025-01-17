namespace HealthCareABApi.DTO
{
    public class CreateAvailabilityDTO
    {
        public CreateAvailabilityDTO(string caregiverId, List<DateTime> availableSlots)
        {
            CaregiverId = caregiverId;
            AvailableSlots = availableSlots;
        }

        private string CaregiverId { get; set; }

        private List<DateTime> AvailableSlots { get; set; }
    }
}

