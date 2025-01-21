using System;
namespace HealthCareABApi.DTO
{
    public class CreateAvailabilityDTO
    {
        public CreateAvailabilityDTO(string caregiverId, List<DateTime> availableSlots)
        {
            CaregiverId = caregiverId;
            AvailableSlots = availableSlots;
        }
        public string CaregiverId { get; private set; }
        public List<DateTime> AvailableSlots { get; private set; }

    }
}

