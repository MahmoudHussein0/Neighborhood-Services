import { Injectable } from '@angular/core';
import{MessageSelectedDto} from '../../../core/models/message-selected-dto';
import { MessageDto } from '../../../core/models/message-dto';
import { ApiService } from '../../../core/services/api.service';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.prod';
import { HttpClient } from '@angular/common/http';




@Injectable({
  providedIn: 'root',
})
export class MessagesService {
  private Endpoint = '/Message';
  constructor(private apiService: ApiService, private http:HttpClient) { }
  messages?: MessageSelectedDto[];

  getMessagesForBooking(bookingId: number) :Observable<MessageSelectedDto[]>{
    return this.apiService.get<MessageSelectedDto[]>(`${this.Endpoint}/booking/${bookingId}`);
  }

  CreateMessagesOnBooking(message: MessageDto): Observable<any>{
    return this.apiService.post(this.Endpoint, message);
  }

  GetCurrentUserId(): Observable<any>{
    return this.apiService.get<{ userId:string }>(this.Endpoint + '/CurrentUserId');
  }

   getCurrentUserId1(): Observable<any> {
    return this.http.get(`https://localhost:7228/api/Message/CurrentUserId`, { responseType: 'text' });
  }

//   getCurrentUserId1(): void {
//   this.http.get(`https://localhost:7228/api/Message/CurrentUserId`, { responseType: 'text' })
//     .subscribe({
//       next: (raw) => console.log('Raw response:', raw),
//       error: (err) => console.error('Error:', err)
//     });
// }
}
