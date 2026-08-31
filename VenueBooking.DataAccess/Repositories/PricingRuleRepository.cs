using VenueBooking.DataAccess.Data;
using VenueBooking.Domain.Interfaces.Repositories;
using VenueBooking.Domain.Models;

namespace VenueBooking.DataAccess.Repositories;

public class PricingRuleRepository(VenueBookingContext context)
    : BaseRepository<PricingRule>(context), IPricingRuleRepository;
