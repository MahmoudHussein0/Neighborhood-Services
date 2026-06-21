import { SupportMessage } from './support-message.model';

export interface SupportTicketDetails {
  id: number;
  userId?: string;
  bookingId?: number;
  subject: string;
  description: string;
  senderName: string;
  senderEmail: string;
  priority: string;
  status: string;
  createdAt: string;
  updatedAt: string;
  messages: SupportMessage[];
}