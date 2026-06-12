import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { ChatbotService, ChatSession } from '../../../core/services/chatbot.service';
import { AuthService } from '../../../features/auth/services/auth.service';
import { UploadService } from '../../services/upload.service';

interface UiMessage {
  role: 'User' | 'Assistant';
  content: string;
  imageUrl?: string;
}

@Component({
  selector: 'app-chat-widget',
  imports: [FormsModule, TranslatePipe],
  templateUrl: './chat-widget.component.html',
  styleUrl: './chat-widget.component.css',
})
export class ChatWidgetComponent {
  private readonly chatbot = inject(ChatbotService);
  private readonly auth = inject(AuthService);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);
  private readonly uploadService = inject(UploadService);

  isOpen = signal(false);
  view = signal<'chat' | 'history'>('chat');

  messages = signal<UiMessage[]>([]);
  draft = '';
  sending = signal(false);

  // Optional photo of the problem, attached to the next message (vision).
  attachedImage = signal<string | null>(null);
  uploadingImage = signal(false);

  // null = no saved session yet (guest, or a fresh chat). Logged-in users get a real id back.
  sessionId = signal<number | null>(null);

  sessions = signal<ChatSession[]>([]);
  loadingSessions = signal(false);

  // Captured once when the user taps "share location"; sent with messages so the
  // backend can resolve their region for localized price estimates.
  coords = signal<{ lat: number; lng: number } | null>(null);

  readonly isAuthenticated = computed(() => this.auth.isAuthenticated());

  toggle() {
    this.isOpen.update((v) => !v);
  }

  close() {
    this.isOpen.set(false);
  }

  onImageSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    input.value = ''; // allow re-selecting the same file
    if (!file) return;

    this.uploadingImage.set(true);
    this.uploadService.upload(file).subscribe({
      next: (url) => {
        this.attachedImage.set(url);
        this.uploadingImage.set(false);
      },
      error: () => {
        this.uploadingImage.set(false);
        this.toastr.error(this.translate.instant('chatbot.imageFailed'));
      },
    });
  }

  removeImage() {
    this.attachedImage.set(null);
  }

  send() {
    const text = this.draft.trim();
    const image = this.attachedImage();
    if ((!text && !image) || this.sending() || this.uploadingImage()) return;

    this.messages.update((m) => [...m, { role: 'User', content: text, imageUrl: image ?? undefined }]);
    this.draft = '';
    this.attachedImage.set(null);
    this.sending.set(true);

    const c = this.coords();
    this.chatbot
      .sendMessage({
        sessionId: this.sessionId(),
        message: text,
        latitude: c?.lat ?? null,
        longitude: c?.lng ?? null,
        imageUrl: image,
      })
      .subscribe({
        next: (r) => {
          this.messages.update((m) => [...m, { role: 'Assistant', content: r.reply }]);
          // 0 = guest (nothing persisted) — keep sessionId null so we never echo 0 back.
          if (r.sessionId && r.sessionId > 0) this.sessionId.set(r.sessionId);
          this.sending.set(false);
        },
        error: () => {
          this.messages.update((m) => [
            ...m,
            { role: 'Assistant', content: this.translate.instant('chatbot.error') },
          ]);
          this.sending.set(false);
        },
      });
  }

  newChat() {
    this.sessionId.set(null);
    this.messages.set([]);
    this.view.set('chat');
  }

  openHistory() {
    if (!this.isAuthenticated()) return;
    this.view.set('history');
    this.loadingSessions.set(true);
    this.chatbot.getSessions().subscribe({
      next: (s) => {
        this.sessions.set(s ?? []);
        this.loadingSessions.set(false);
      },
      // 401 (expired token) or any failure → just drop back to the chat view.
      error: () => {
        this.loadingSessions.set(false);
        this.view.set('chat');
      },
    });
  }

  backToChat() {
    this.view.set('chat');
  }

  loadSession(id: number) {
    this.chatbot.getSession(id).subscribe({
      next: (d) => {
        this.sessionId.set(d.id);
        this.messages.set((d.messages ?? []).map((m) => ({ role: m.role, content: m.content })));
        this.view.set('chat');
      },
    });
  }

  shareLocation() {
    if (!navigator.geolocation) {
      this.toastr.error(this.translate.instant('common.geoUnsupported'));
      return;
    }
    navigator.geolocation.getCurrentPosition(
      (pos) => {
        this.coords.set({ lat: pos.coords.latitude, lng: pos.coords.longitude });
        // Make it visible in the chat + tell the user what to do next — the coords
        // are attached to their next message, not sent on their own.
        this.messages.update((m) => [
          ...m,
          { role: 'Assistant', content: this.translate.instant('chatbot.locationShared') },
        ]);
      },
      () =>
        this.messages.update((m) => [
          ...m,
          { role: 'Assistant', content: this.translate.instant('chatbot.locationDenied') },
        ]),
    );
  }

  clearLocation() {
    this.coords.set(null);
  }
}
