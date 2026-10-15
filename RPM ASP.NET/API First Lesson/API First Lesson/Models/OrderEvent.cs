using System;

namespace API_First_Lesson.Models;

public partial class OrderEvent
{
    public const string TypeReserve = "Reserve";

    public const string TypeCancelled = "Cancelled";

    public int OrderEventId { get; set; }

    public int? OrderId { get; set; }

    public int? UserId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public string EventType { get; set; } = null!;

    public DateTime EventDate { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User? User { get; set; }
}
