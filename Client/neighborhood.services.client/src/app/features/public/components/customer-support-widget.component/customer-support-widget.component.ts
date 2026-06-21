import { Component, ElementRef, ViewChild, AfterViewChecked, OnDestroy, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Subject, takeUntil } from 'rxjs';

import { CustomerSupportTicketService } from '../../services/customer-support-ticket.service';
import { AuthService } from '../../../auth/services/auth.service';

import { SupportMessagesService } from '../../../staff/services/support-messages.service';
import { ChatSignalRService, ChatMessage } from '../../../staff/services/chat-signalr.service';
import { AttachmentUploadService } from '../../../staff/services/AttachmentUpload.service';
import { AttachmentType, MessageChannel, SupportMessage } from '../../../staff/models/support-message.model';
import { SupportTicket } from '../../../staff/models/support-ticket.model';

type WidgetView = 'form' | 'chat';

@Component({
  selector: 'app-customer-support-widget',
  standalone: true,
  imports: [FormsModule, TranslatePipe, DatePipe],
  templateUrl: './customer-support-widget.component.html',
  styleUrl: './customer-support-widget.component.css',
})
export class CustomerSupportWidgetComponent implements AfterViewChecked, OnDestroy {
  @ViewChild('scrollAnchor') private scrollAnchor?: ElementRef<HTMLDivElement>;

  private readonly ticketService = inject(CustomerSupportTicketService);
  private readonly messagesService = inject(SupportMessagesService);
  private readonly uploadService = inject(AttachmentUploadService);
  private readonly chat = inject(ChatSignalRService);
  private readonly auth = inject(AuthService);
  private readonly translate = inject(TranslateService);

  isOpen = signal(false);
  view = signal<WidgetView>('form');

  // ── New ticket form ───────────────────────────────────────────────────
  subject = '';
  description = '';
  senderName = '';
  senderEmail = '';
  submitting = signal(false);
  formError = signal<string | null>(null);

  readonly isAuthenticated = computed(() => this.auth.isAuthenticated());

  // ── Active ticket / chat ──────────────────────────────────────────────
  ticket = signal<SupportTicket | null>(null);
  messages = signal<SupportMessage[]>([]);
  draft = '';
  sending = signal(false);
  connectionState = signal<'connected' | 'disconnected' | 'reconnecting'>('disconnected');
  private shouldScrollToBottom = false;

  pendingAttachment = signal<{ url: string; publicId: string; type: AttachmentType; previewUrl: string } | null>(null);
  uploadingAttachment = signal(false);
  uploadError = signal<string | null>(null);

  lightboxUrl = signal<string | null>(null);
  lightboxIsVideo = signal(false);

  private destroy$ = new Subject<void>();

  constructor() {
    // Pre-fill name/email for logged-in customers so they don't have to retype it.
    const user = this.auth.currentUser();
    if (user) {
      this.senderName = user.fullName ?? '';
      this.senderEmail = user.email ?? '';
    }
  }

  ngAfterViewChecked() {
    if (this.shouldScrollToBottom) {
      this.scrollAnchor?.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'end' });
      this.shouldScrollToBottom = false;
    }
  }

  ngOnDestroy() {
    this.leaveChat();
    this.destroy$.next();
    this.destroy$.complete();
  }

  toggle() {
    this.isOpen.update(v => !v);
  }

  close() {
    this.isOpen.set(false);
  }

  // ── Step 1: create the ticket ────────────────────────────────────────

  submitTicket() {
    this.formError.set(null);

    if (!this.subject.trim() || !this.description.trim() || !this.senderName.trim() || !this.senderEmail.trim()) {
      this.formError.set(this.translate.instant('supportWidget.fillAllFields'));
      return;
    }

    this.submitting.set(true);
    this.ticketService.createTicket({
      subject: this.subject.trim(),
      description: this.description.trim(),
      senderName: this.senderName.trim(),
      senderEmail: this.senderEmail.trim(),
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: (created) => {
        this.submitting.set(false);
        this.ticket.set(created);
        this.view.set('chat');
        this.connectChat(created.id);
      },
      error: () => {
        this.submitting.set(false);
        this.formError.set(this.translate.instant('supportWidget.submitFailed'));
      },
    });
  }

  // ── Step 2: chat ──────────────────────────────────────────────────────

  private connectChat(ticketId: number) {
    this.messagesService.getMessages(ticketId).pipe(takeUntil(this.destroy$)).subscribe({
      next: (msgs) => {
        this.messages.set(
          [...(msgs ?? [])].sort((a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime())
        );
        this.shouldScrollToBottom = true;
      },
    });

    this.chat.startConnection()
      .then(() => this.chat.joinTicket(ticketId))
      .catch(() => {});

    this.chat.connectionState$.pipe(takeUntil(this.destroy$)).subscribe(state => this.connectionState.set(state));

    this.chat.message$.pipe(takeUntil(this.destroy$)).subscribe((msg: ChatMessage) => {
      if (msg.ticketId !== ticketId) return;
      const current = this.messages();
      if (msg.id != null && current.some(m => m.id === msg.id)) return;
      this.messages.set([...current, msg as SupportMessage]);
      this.shouldScrollToBottom = true;
    });
  }

  private leaveChat() {
    const id = this.ticket()?.id;
    if (id != null) this.chat.leaveTicket(id).catch(() => {});
    this.chat.stopConnection();
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    input.value = '';
    if (!file) return;

    if (file.size > 25 * 1024 * 1024) {
      this.uploadError.set(this.translate.instant('supportWidget.fileTooLarge'));
      return;
    }

    const isImage = file.type.startsWith('image/');
    const isVideo = file.type.startsWith('video/');
    if (!isImage && !isVideo) {
      this.uploadError.set(this.translate.instant('supportWidget.unsupportedFile'));
      return;
    }
    const type: AttachmentType = isImage ? AttachmentType.Image : AttachmentType.Video;

    this.uploadingAttachment.set(true);
    this.uploadError.set(null);

    this.uploadService.upload(file).subscribe({
      next: ({ url, publicId }) => {
        this.pendingAttachment.set({ url, publicId, type, previewUrl: url });
        this.uploadingAttachment.set(false);
      },
      error: () => {
        this.uploadingAttachment.set(false);
        this.uploadError.set(this.translate.instant('supportWidget.uploadFailed'));
      },
    });
  }

  removePendingAttachment() {
    this.pendingAttachment.set(null);
  }

  sendMessage() {
    const text = this.draft.trim();
    const attachment = this.pendingAttachment();
    const ticketId = this.ticket()?.id;
    if ((!text && !attachment) || !ticketId) return;
    if (this.sending() || this.uploadingAttachment()) return;
    if (this.ticket()?.status === 'Resolved') return;

    this.sending.set(true);
    this.uploadError.set(null);

    this.messagesService.sendMessage(ticketId, {
      ticketId,
      message: text,
      channel: MessageChannel.Chat,
      attachments: attachment
        ? [{ url: attachment.url, publicId: attachment.publicId, type: attachment.type }]
        : undefined,
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: (saved) => {
        this.sending.set(false);
        this.draft = '';
        this.pendingAttachment.set(null);

        const broadcastPayload: ChatMessage = {
          id: saved.id,
          ticketId: saved.ticketId,
          senderId: saved.senderId,
          senderType: saved.senderType,
          message: saved.message,
          channel: saved.channel,
          createdAt: saved.createdAt,
          attachments: saved.attachments,
        };
        this.chat.sendMessage(ticketId, broadcastPayload).catch(() => {});

        const current = this.messages();
        if (!current.some(m => m.id === saved.id)) {
          this.messages.set([...current, saved]);
          this.shouldScrollToBottom = true;
        }
      },
      error: () => this.sending.set(false),
    });
  }

  isStaffMessage(msg: SupportMessage): boolean {
    return msg.senderType === 'Staff';
  }

  showSenderHeader(index: number): boolean {
    const msgs = this.messages();
    if (index === 0) return true;
    return this.isStaffMessage(msgs[index]) !== this.isStaffMessage(msgs[index - 1]);
  }

  openLightbox(url: string, type: string) {
    this.lightboxUrl.set(url);
    this.lightboxIsVideo.set(type === 'Video');
  }

  closeLightbox() {
    this.lightboxUrl.set(null);
  }

  /** Start an entirely new ticket from inside an active chat. */
  startNewTicket() {
    this.leaveChat();
    this.ticket.set(null);
    this.messages.set([]);
    this.subject = '';
    this.description = '';
    this.formError.set(null);
    this.view.set('form');
  }

  getStatusLabel(status: string): string {
    const map: Record<string, string> = {
      Open: 'Open', InProgress: 'In progress',
      WaitingOnCustomer: 'Waiting on you', Resolved: 'Resolved',
    };
    return map[status] ?? status;
  }
}