using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Offers.Commands.Withdraw
{
    public class WithdrawOfferCommand:IRequest<bool>
    {
        public int OfferId { get; set; }
    }
}
