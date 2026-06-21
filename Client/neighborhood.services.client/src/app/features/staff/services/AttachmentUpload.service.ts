import { Injectable, inject } from '@angular/core';
import { HttpBackend, HttpClient } from '@angular/common/http';
import { Observable, switchMap, map } from 'rxjs';
import { CloudinaryService } from './cloudinary.service';

interface CloudinarySignature {
  signature: string;
  timestamp: number;
  apiKey: string;
  cloudName: string;
}

interface CloudinaryUploadResponse {
  secure_url: string;
  public_id: string;
}

/**
 * Uploads a file straight to Cloudinary using a signature minted by our own
 * backend (CloudinaryService -> POST /api/files/signature). Used for chat
 * attachments in the support tickets thread.
 */
@Injectable({ providedIn: 'root' })
export class AttachmentUploadService {
  private readonly cloudinaryService = inject(CloudinaryService);

  // Raw HttpClient built straight off HttpBackend — bypasses ALL interceptors
  // (auth, error, lang...). The Cloudinary call is cross-origin and must NOT
  // carry our `withCredentials: true` / auth cookies, or Cloudinary's CORS
  // response rejects it (it doesn't send Access-Control-Allow-Credentials).
  private readonly rawHttp = new HttpClient(inject(HttpBackend));

  /** Uploads `file` to Cloudinary and resolves to its secure URL + public id. */
  upload(file: File): Observable<{ url: string; publicId: string }> {
    return this.cloudinaryService.getSignature().pipe(
      switchMap((sig: any) => {
        const signature = sig as CloudinarySignature;
        const form = new FormData();
        form.append('file', file);
        form.append('api_key', signature.apiKey);
        form.append('timestamp', String(signature.timestamp));
        form.append('signature', signature.signature);

        const url = `https://api.cloudinary.com/v1_1/${signature.cloudName}/auto/upload`;
        return this.rawHttp.post<CloudinaryUploadResponse>(url, form);
      }),
      map((res) => ({ url: res.secure_url, publicId: res.public_id })),
    );
  }
}