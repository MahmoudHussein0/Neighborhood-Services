import { Injectable } from '@angular/core';
import{MessageSelectedDto} from '../../../core/models/message-selected-dto';
import { MessageDto } from '../../../core/models/message-dto';
import { ApiService } from '../../../core/services/api.service';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root',
})
export class MessagesService {
  private Endpoint = '/api/Message';
  constructor(private apiService: ApiService) { }
  messages?: MessageSelectedDto[];

  getMessagesForBooking(bookingId: number) :Observable<MessageSelectedDto[]>{
    return this.apiService.get<MessageSelectedDto[]>(`${this.Endpoint}/booking/${bookingId}`);
  }

  CreateMessagesOnBooking(message: MessageDto): Observable<any>{
    return this.apiService.post(this.Endpoint, message);
  }

  GetCurrentUserId(): Observable<any>{
    return this.apiService.get(this.Endpoint + '/GetCurrentUserId');
  }
}
