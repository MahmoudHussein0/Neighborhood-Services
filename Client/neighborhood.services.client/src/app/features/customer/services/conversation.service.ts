import { Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import {ConversationDto} from '../models/conversation-dto'
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root',
})
export class ConversationService {

  private Endpoint = '/Conversation';
  constructor(private apiService: ApiService) { }
  convs?: ConversationDto[];

  getMyConversations():Observable<ConversationDto[]> {
     return this.apiService.get<ConversationDto[]>(this.Endpoint + '/GetForCurrentUser');
    }
}
