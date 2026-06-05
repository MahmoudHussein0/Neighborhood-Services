using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Services.NotificationService
{
    public class CreateNotificationService
    {
        //booking,
        public Notification GenBookingNotfStaff(Booking booking)
        {
            return new Notification()
            {
                // UserId=booking.Technician.ApplicationUserId,
                type = NotificationTypes.booking,
                message = $"A new booking, from customer with Id:{booking.CustomerId}" +
                $"name:{booking} to technician {booking.TechnicianId} "
                ,
                channel = NotificationChannels.push,
                createdAt=DateTime.UtcNow,
                

               
            };
        }

        public Notification GenBookingNotfCustomer(Booking booking)
        {
            return new Notification()
            {
                type = NotificationTypes.booking,
                message = $"Your booking is verified! "
                ,
                channel = NotificationChannels.push,
                createdAt = DateTime.UtcNow,


            };
        }

        public Notification GenBookingNotfTechnician(Booking booking)
        {
            return new Notification()
            {
                type = NotificationTypes.booking,
                message = $"You have a new booking request from client {booking.CustomerId}" +
                $"on service of type {booking.ProblemType.NameEn} at Address {booking.Location.ToString()} "
                ,
                channel = NotificationChannels.push,
                createdAt = DateTime.UtcNow,

            };
        }

        //Example:
        /*After Booking Reservation:
         * 
         * booking object is created assume it is bk
         * 
         * var not1= GenBookingNotfStaff(bk;)
         * 
         * _notificationsReposotory.Add + save changes
         * 
         * await _notificationService.SendRoleBasedNotificationAsync(not1.Message,role: "Staff");

         * 
         * 
         * 
         * 
         * */



        //payment,
        //review,
        //dispute,
        //support,
        //system,
        //general


    }
}
