using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManager.Shared.DTOs;

public class CreateSubscriptionDto
{
    public Guid ApplicationId { get; set; }
    public List<Guid> PlaceIds { get; set; } = new();
}
