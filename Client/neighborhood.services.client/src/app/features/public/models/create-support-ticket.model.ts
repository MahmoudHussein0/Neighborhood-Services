// Body for POST /api/supporttickets — used by both guests and logged-in customers
// (SenderName/SenderEmail are entered manually, no auth required to open a ticket).
export interface CreateSupportTicket {
  subject: string;
  description: string;
  senderName: string;
  senderEmail: string;
}