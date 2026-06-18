using ETS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETS.Domain.Errors
{
    public class TicketError
    {
        public static readonly Error NotFound = new("Ticket.NotFound", "The ticket was not found");
        public static readonly Error SomethingWentWrong = new("Event.Exception", "Something went wrong, pls try again later");
    }
}
