using System;
using System.Collections.Generic;
using System.Text;
using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Domain.Message;


namespace Neighborhood.Services.Domain.Conversation
{
    public class Conversation :BaseEntity<int>
    {
       //Foriegn Key
       /// <summary>
       /// updattteee
       /// </summary>
        public int BookingId { get; set; }

        //Foriegn key
        public int ServiceRequestId { set; get; }
        public DateTime createdAt { get; set; }
        public Message.Message lastMessage => Messages.ToList().Last()??new Message.Message();



        //Nav propbs:
        //public ServiceRequest ServiceRequest {set; get;}
        //public Booking Booking {set; get;}
        public ICollection <Message.Message> Messages= new List<Message.Message> ();
    }
}
