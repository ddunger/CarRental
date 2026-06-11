using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Domain.Enums
{
    public enum ReservationStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        NoShow,
        Converted
    }
}
