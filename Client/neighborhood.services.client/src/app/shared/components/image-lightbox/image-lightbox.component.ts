import { Component, inject } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

/**
 * Fullscreen floating image viewer. Opened via LightboxService.open(url).
 * Click anywhere (or the close button / Esc / backdrop) to dismiss.
 */
@Component({
  selector: 'app-image-lightbox',
  template: `
    <button type="button" class="ns-lightbox-close" (click)="close()" aria-label="Close">
      <i class="bi bi-x-lg"></i>
    </button>
    <div class="ns-lightbox-body" (click)="close()">
      <img [src]="src" alt="" class="ns-lightbox-img" (click)="$event.stopPropagation()" />
    </div>
  `,
  styles: [`
    :host { display: block; position: relative; }
    .ns-lightbox-body {
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: zoom-out;
      padding: 0.5rem;
    }
    .ns-lightbox-img {
      max-width: 100%;
      max-height: 82vh;
      border-radius: 0.5rem;
      box-shadow: 0 1rem 3rem rgba(0, 0, 0, 0.45);
      cursor: default;
    }
    .ns-lightbox-close {
      position: absolute;
      top: -0.75rem;
      inset-inline-end: -0.75rem;
      z-index: 2;
      width: 2rem;
      height: 2rem;
      border: none;
      border-radius: 50%;
      background: #fff;
      color: #1e293b;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
      display: flex;
      align-items: center;
      justify-content: center;
    }
  `],
})
export class ImageLightboxComponent {
  private readonly activeModal = inject(NgbActiveModal);
  src = '';
  close() {
    this.activeModal.dismiss();
  }
}
