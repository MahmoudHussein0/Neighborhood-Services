import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

// Body for POST /api/chatbot. sessionId null = start a new chat; otherwise continue.
export interface SendChatMessageRequest {
  sessionId: number | null;
  message: string;
  region?: string | null;
  latitude?: number | null;
  longitude?: number | null;
  imageUrl?: string | null;   // optional photo of the problem (vision)
}

// Mirrors ChatReplyDto
export interface ChatReply {
  sessionId: number; // 0 for guests (nothing persisted)
  reply: string;
}

// Mirrors ChatSessionDto (GET /api/chatbot/sessions)
export interface ChatSession {
  id: number;
  title: string | null;
  createdAt: string;
}

// Mirrors ChatMessageDto
export interface ChatMessage {
  role: 'User' | 'Assistant';
  content: string;
  createdAt: string;
}

// Mirrors ChatSessionDetailDto (GET /api/chatbot/sessions/{id})
export interface ChatSessionDetail {
  id: number;
  title: string | null;
  createdAt: string;
  messages: ChatMessage[];
}

@Injectable({ providedIn: 'root' })
export class ChatbotService {
  private readonly api = inject(ApiService);

  /** POST /api/chatbot — send a message (works for guests and logged-in users). */
  sendMessage(body: SendChatMessageRequest): Observable<ChatReply> {
    return this.api.post<ChatReply>('/chatbot', body);
  }

  /** GET /api/chatbot/sessions — the current user's past chats (login required). */
  getSessions(): Observable<ChatSession[]> {
    return this.api.get<ChatSession[]>('/chatbot/sessions');
  }

  /** GET /api/chatbot/sessions/{id} — one chat with its full message history. */
  getSession(id: number): Observable<ChatSessionDetail> {
    return this.api.get<ChatSessionDetail>(`/chatbot/sessions/${id}`);
  }
}
