import {
  Component, Input, Output, EventEmitter,
  OnChanges, SimpleChanges, OnDestroy, AfterViewChecked,
  ChangeDetectionStrategy, ChangeDetectorRef, ElementRef, ViewChild,
  inject
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';

import { SupportTicketsService } from '../../services/support-ticket.service';
import { SupportMessagesService } from '../../services/support-messages.service';
import { ChatSignalRService, ChatMessage } from '../../services/chat-signalr.service';
import { AttachmentUploadService } from '../../services/AttachmentUpload.service';  
import { AuthService } from '../../../auth/services/auth.service';

import { SupportTicketDetails } from '../../models/support-ticket-details.model';
import { AttachmentType, MessageChannel, SupportMessage } from '../../models/support-message.model';

@Component({
  selector: 'app-ticket-details',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ticket-details.component.html',
  styleUrl: './ticket-details.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TicketDetailsComponent implements OnChanges, OnDestroy, AfterViewChecked {
  @Input() ticketId!: number;
  @Output() ticketUpdated = new EventEmitter<void>();
  @Output() close = new EventEmitter<void>();

  @ViewChild('scrollAnchor') private scrollAnchor?: ElementRef<HTMLDivElement>;

  private readonly ticketService = inject(SupportTicketsService);
  private readonly messagesService = inject(SupportMessagesService);
  private readonly uploadService = inject(AttachmentUploadService);
  private readonly chat = inject(ChatSignalRService);
  private readonly auth = inject(AuthService);
  private readonly cdr = inject(ChangeDetectorRef);

  loading = false;
  ticket: SupportTicketDetails | null = null;

  // ── Chat ──────────────────────────────────────────────────────────────
  messages: SupportMessage[] = [];
  draft = '';
  sending = false;
  connectionState: 'connected' | 'disconnected' | 'reconnecting' = 'disconnected';
  private shouldScrollToBottom = false;

  // Pending attachment for the next outgoing message.
  pendingAttachment: { url: string; publicId: string; type: AttachmentType; name: string; previewUrl: string } | null = null;
  uploadingAttachment = false;
  uploadError: string | null = null;

  // Lightbox for viewing an attachment full-size.
  lightboxUrl: string | null = null;
  lightboxIsVideo = false;

  private readonly currentUserId = this.auth.currentUser()?.userId ?? '';

  private destroy$ = new Subject<void>();

  ngOnChanges(changes: SimpleChanges) {
    if (changes['ticketId'] && this.ticketId) {
      this.resetComposer();
      this.loadDetails();
      this.connectChat();
    }
  }

  ngAfterViewChecked() {
    if (this.shouldScrollToBottom) {
      this.scrollAnchor?.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'end' });
      this.shouldScrollToBottom = false;
    }
  }

  ngOnDestroy() {
    this.chat.leaveTicket(this.ticketId).catch(() => {});
    this.chat.stopConnection();
    this.destroy$.next();
    this.destroy$.complete();
  }

  // ── Loading ───────────────────────────────────────────────────────────

  loadDetails() {
    this.loading = true;
    this.ticket = null;
    this.messages = [];
    this.cdr.markForCheck();

    this.ticketService.getTicketDetails(this.ticketId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.ticket = res;
          this.messages = [...(res.messages ?? [])].sort(
            (a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
          );
          this.loading = false;
          this.shouldScrollToBottom = true;
          this.cdr.markForCheck();
        },
        error: () => {
          this.loading = false;
          this.cdr.markForCheck();
        }
      });
  }

  private async connectChat() {
    try {
      await this.chat.startConnection();
      await this.chat.joinTicket(this.ticketId);
    } catch {
      // connectionState$ below will reflect the failure
    }

    this.chat.connectionState$
      .pipe(takeUntil(this.destroy$))
      .subscribe((state) => {
        this.connectionState = state;
        this.cdr.markForCheck();
      });

    this.chat.message$
      .pipe(takeUntil(this.destroy$))
      .subscribe((msg: ChatMessage) => {
        if (msg.ticketId !== this.ticketId) return;
        if (msg.id != null && this.messages.some(m => m.id === msg.id)) return;
        this.messages = [...this.messages, msg as SupportMessage];
        this.shouldScrollToBottom = true;
        this.cdr.markForCheck();
      });
  }

  // ── Attachments gallery (every image/video shared anywhere in this thread) ──

  get galleryAttachments() {
    return this.messages.flatMap(m =>
      (m.attachments ?? [])
        .filter(a => a.type === 'Image' || a.type === 'Video')
        .map(a => ({ ...a, messageId: m.id }))
    );
  }

  openLightbox(url: string, type: string) {
    this.lightboxUrl = url;
    this.lightboxIsVideo = type === 'Video';
  }

  closeLightbox() {
    this.lightboxUrl = null;
  }

  // ── Sending messages ──────────────────────────────────────────────────

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    input.value = '';
    if (!file) return;

    if (file.size > 25 * 1024 * 1024) {
      this.uploadError = 'File is too large (max 25 MB).';
      this.cdr.markForCheck();
      return;
    }

    const isImage = file.type.startsWith('image/');
    const isVideo = file.type.startsWith('video/');
    if (!isImage && !isVideo) {
      this.uploadError = 'Only images and videos can be attached.';
      this.cdr.markForCheck();
      return;
    }
    const type: AttachmentType = isImage ? AttachmentType.Image : AttachmentType.Video;

    this.uploadingAttachment = true;
    this.uploadError = null;
    this.cdr.markForCheck();

    this.uploadService.upload(file).subscribe({
      next: ({ url, publicId }) => {
        this.pendingAttachment = { url, publicId, type, name: file.name, previewUrl: url };
        this.uploadingAttachment = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.uploadingAttachment = false;
        this.uploadError = "Couldn't upload the file. Please try again.";
        this.cdr.markForCheck();
      },
    });
  }

  removePendingAttachment() {
    this.pendingAttachment = null;
  }

  sendMessage() {
    const text = this.draft.trim();
    if (!text && !this.pendingAttachment) return;
    if (this.sending || this.uploadingAttachment) return;
    if (!this.ticket || this.ticket.status === 'Resolved') return;

    this.sending = true;
    this.uploadError = null;
    const attachment = this.pendingAttachment;

    this.messagesService.sendMessage(this.ticketId, {
      ticketId: this.ticketId,
      message: text,
      channel: MessageChannel.Chat,
      attachments: attachment
        ? [{ url: attachment.url, publicId: attachment.publicId, type: attachment.type }]
        : undefined,
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: (saved) => {
        this.sending = false;
        this.resetComposer();
        // Broadcast the now-persisted message (with its real id + attachments)
        // to everyone else viewing this ticket. We'll also receive it back
        // ourselves through message$, but the id-dedup below prevents a double bubble.
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
        this.chat.sendMessage(this.ticketId, broadcastPayload).catch(() => {});

        if (!this.messages.some(m => m.id === saved.id)) {
          this.messages = [...this.messages, saved];
          this.shouldScrollToBottom = true;
        }
        this.cdr.markForCheck();
      },
      error: () => {
        this.sending = false;
        this.cdr.markForCheck();
      },
    });
  }

  private resetComposer() {
    this.draft = '';
    this.pendingAttachment = null;
    this.uploadError = null;
  }

  isStaffMessage(msg: SupportMessage): boolean {
    return msg.senderType === 'Staff';
  }

  /** Whether to show the sender name/avatar above this bubble (collapses consecutive messages). */
  showSenderHeader(index: number): boolean {
    if (index === 0) return true;
    return this.isStaffMessage(this.messages[index]) !== this.isStaffMessage(this.messages[index - 1]);
  }

  // ── Ticket actions (status / priority / delete) ─────────────────────────

  updateStatus(newStatus: number) {
    this.ticketService.updateTicketStatus(this.ticketId, newStatus)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          if (this.ticket) this.ticket = { ...this.ticket, status: this.mapStatus(newStatus) };
          this.ticketUpdated.emit();
          this.cdr.markForCheck();
        }
      });
  }

  changePriority(priority: string) {
    const map: Record<string, number> = { High: 0, Medium: 1, Low: 2 };
    this.ticketService.updateTicketPriority(this.ticketId, map[priority])
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          if (this.ticket) this.ticket = { ...this.ticket, priority };
          this.ticketUpdated.emit();
          this.cdr.markForCheck();
        }
      });
  }

  deleteTicket() {
    if (!confirm('Delete this ticket? This action cannot be undone.')) return;
    this.ticketService.deleteTicket(this.ticketId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.ticketUpdated.emit();
          this.close.emit();
          this.ticket = null;
          this.cdr.markForCheck();
        }
      });
  }

  copyEmail(email: string) {
    navigator.clipboard.writeText(email).catch(() => {});
  }

  getInitials(name: string): string {
    if (!name) return '?';
    return name.trim()
      .split(/\s+/)
      .map(w => w[0])
      .slice(0, 2)
      .join('')
      .toUpperCase();
  }

  private mapStatus(status: number): string {
    return ['Open', 'InProgress', 'WaitingOnCustomer', 'Resolved'][status] ?? 'Open';
  }

  getStatusLabel(status: string): string {
    const map: Record<string, string> = {
      Open: 'Open',
      InProgress: 'In progress',
      WaitingOnCustomer: 'Waiting on customer',
      Resolved: 'Resolved',
    };
    return map[status] ?? status;
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      Open: 'badge-open',
      InProgress: 'badge-inprogress',
      WaitingOnCustomer: 'badge-waiting',
      Resolved: 'badge-resolved',
    };
    return map[status] ?? 'badge-neutral';
  }

  getPriorityClass(priority: string): string {
    const map: Record<string, string> = {
      High: 'badge-high',
      Medium: 'badge-medium',
      Low: 'badge-low',
    };
    return map[priority] ?? 'badge-neutral';
  }
}