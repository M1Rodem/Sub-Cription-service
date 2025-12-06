using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManager.Shared.DTOs;

public class UpdateSubscriptionDto
{
    public int? Status { get; set; }
    public List<Guid>? PlaceIds { get; set; }
}
