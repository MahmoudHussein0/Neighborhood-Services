export class TicketDto {

 public subject!:string;
  public description!:string
  public senderName!:string
  public  senderEmail!:string
  public localId: string=`ticket-${Date.now()}-${Math.random().toString(36).slice(2)}`

}
