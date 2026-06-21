export interface SupportTicket {
  id: number;
  userId?: string;

  senderName: string;
  senderEmail: string;

  bookingId?: number;

  subject: string;
  description: string;

  status: string;
  priority: string;

  createdAt: string;
  updatedAt: string;
}