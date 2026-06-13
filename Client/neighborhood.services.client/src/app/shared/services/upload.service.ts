import { Injectable, inject } from '@angular/core';
import { HttpBackend, HttpClient } from '@angular/common/http';
import { Observable, switchMap, map } from 'rxjs';
import { ApiService } from '../../core/services/api.service';

// Mirrors CloudinarySignatureDto from POST /api/files/signature
interface CloudinarySignature {
  signature: string;
  timestamp: number;
  apiKey: string;
  cloudName: string;
}

// Subset of Cloudinary's upload response we care about
interface CloudinaryUploadResponse {
  secure_url: string;
  public_id: string;
}

/**
 * Shared image uploader (used by Booking images, Technician photos, Profile pic).
 * Flow: get a signature from our backend -> upload the file straight to Cloudinary -> return the URL.
 */
@Injectable({
  providedIn: 'root',
})
export class UploadService {
  // Signature call goes through our normal pipeline (auth + error interceptors)
  private readonly api = inject(ApiService);

  // Raw HttpClient that does NOT pass through our interceptors —
  // the Cloudinary call is cross-origin and must not carry our cookies / error rules.
  private readonly rawHttp = new HttpClient(inject(HttpBackend));

  /** Uploads a file to Cloudinary and resolves to its secure URL. */
  upload(file: File): Observable<string> {
    return this.api.post<CloudinarySignature>('/files/signature', {}).pipe(
      switchMap((sig) => {
        // Only the params the backend signed (just timestamp) — nothing extra.
        const form = new FormData();
        form.append('file', file);
        form.append('api_key', sig.apiKey);
        form.append('timestamp', String(sig.timestamp));
        form.append('signature', sig.signature);

        const url = `https://api.cloudinary.com/v1_1/${sig.cloudName}/image/upload`;
        return this.rawHttp.post<CloudinaryUploadResponse>(url, form);
      }),
      map((res) => res.secure_url)
    );
  }
}
