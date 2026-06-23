import { Injectable, inject } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ImageLightboxComponent } from '../components/image-lightbox/image-lightbox.component';

/**
 * Opens any image URL in a floating fullscreen viewer.
 * Usage: inject LightboxService, then `(click)="lightbox.open(url)"` on a thumbnail.
 */
@Injectable({ providedIn: 'root' })
export class LightboxService {
  private readonly modal = inject(NgbModal);

  open(src: string | null | undefined) {
    if (!src) return;
    const ref = this.modal.open(ImageLightboxComponent, {
      centered: true,
      size: 'lg',
      windowClass: 'ns-lightbox-window',
    });
    ref.componentInstance.src = src;
  }
}
