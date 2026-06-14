export class MessageSelectedDto {

public messageId?: number;
public conversationId?: number;
public senderId?: string
public senderName?: string;
public content?: string;
public read?: boolean;
public deleted?: boolean;
public sentAt: Date = new Date();


    // public id?: number;
    // public senderId?: string;
    // public content!: string;
    // public bookingId!: number;

constructor(init?: Partial<MessageSelectedDto>) {
    Object.assign(this, init);
  }
}

