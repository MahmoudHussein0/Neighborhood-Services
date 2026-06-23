export class MessageDto {

// public int id {  get; set; }
// public string scss_msg = "Message Created Successfully!";
// public string senderId { set; get; }
// public string content { set; get; }

// public int BookingId { set; get; }


    public id?: number;
    public senderId?: string;
    public content!: string;
    public bookingId!: number;
    public hasImage?:boolean;
    public imageUrl?:string;

}
