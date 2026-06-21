// Mirrors MessageChannel (Domain enum) — sent as a number, serialized as a string by the API.
export enum MessageChannel {
  Chat = 1,
  Mail = 2,
}

// Mirrors AttachmentType (Domain enum) — sent as a number, serialized as a string by the API.
export enum AttachmentType {
  Image = 1,
  Video = 2,
}

export interface Attachment {
  id: number;
  url: string;
  publicId: string;
  type: string; // comes back as the enum's string name, e.g. "Image" | "Video"
}

export interface SupportMessage {
  id: number;
  ticketId: number;
  senderId?: string;
  senderType: string; // "Staff" | "user"
  message?: string;
  channel: string;
  createdAt: string;
  readAt?: string;
  attachments: Attachment[];
}

export interface CreateAttachment {
  url: string;
  publicId: string;
  type: AttachmentType;
}

export interface CreateSupportMessage {
  ticketId: number;
  message?: string;
  channel: MessageChannel;
  attachments?: CreateAttachment[];
}