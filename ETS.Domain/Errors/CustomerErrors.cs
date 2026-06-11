using ETS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETS.Domain.Errors
{
    public class CustomerErrors
    {
        public static readonly Error InvalidEventId = new("Customer.EventNotFound", "The event was not found");
    }
}
