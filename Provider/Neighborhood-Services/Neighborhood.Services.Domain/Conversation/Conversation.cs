using System;
using System.Collections.Generic;
using System.Text;
using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Domain.Message;
using Neighborhood.Services.Domain.ServiceRequests;
using Neighborhood.Services.Domain.Bookings;


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
        public ServiceRequest ServiceRequest { get; set; } =new ServiceRequest();
        //public Booking Booking {set; get;}
        public Booking Booking {  get; set; }=new Booking();
        public ICollection <Message.Message> Messages { get; set; } = new List<Message.Message> ();
    }
}
