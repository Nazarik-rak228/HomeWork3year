using System.Collections.Generic;

namespace API_First_Lesson.Models;

public static class OrderStatuses
{
    public const string Pending = "Pending";

    public const string Paid = "Paid";

    public const string Shipped = "Shipped";

    public const string Completed = "Completed";

    public const string Cancelled = "Cancelled";

    public static readonly IReadOnlyList<string> All =
        new List<string> { Pending, Paid, Shipped, Completed, Cancelled };
}
