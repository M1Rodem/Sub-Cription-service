using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManager.Shared.DTOs;

public class SubscriptionDto
{
    public Guid Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Status { get; set; }
    public Guid UserId { get; set; }
    public Guid ApplicationId { get; set; }
    public List<Guid> PlaceIds { get; set; } = new();
}