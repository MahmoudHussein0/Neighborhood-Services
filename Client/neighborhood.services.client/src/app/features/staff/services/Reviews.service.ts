import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  ReviewDto,
  CreateReviewRequest,
  UpdateReviewRequest,
  ReviewStatus,
}
from '../models/Review.model';
import { ApiService } from '../../../core/services/api.service';

@Injectable({ providedIn: 'root' })
export class ReviewsService {
  private readonly api = inject(ApiService);
  // ApiService already prepends `${environment.apiUrl}/api`, so paths are relative.
  private readonly path = '/reviews';

  getAll(): Observable<ReviewDto[]> {
    return this.api.get<ReviewDto[]>(this.path);
  }

  getById(id: number): Observable<ReviewDto> {
    return this.api.get<ReviewDto>(`${this.path}/${id}`);
  }

  create(payload: CreateReviewRequest): Observable<ReviewDto> {
    return this.api.post<ReviewDto>(this.path, payload);
  }

  update(id: number, payload: UpdateReviewRequest): Observable<ReviewDto> {
    return this.api.put<ReviewDto>(`${this.path}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.api.delete<void>(`${this.path}/${id}`);
  }

  getByReviewer(reviewerId: string): Observable<ReviewDto[]> {
    return this.api.get<ReviewDto[]>(`${this.path}/reviewer/${reviewerId}`);
  }

  getByReviewee(revieweeId: string): Observable<ReviewDto[]> {
    return this.api.get<ReviewDto[]>(`${this.path}/reviewee/${revieweeId}`);
  }

  getByStatus(status: ReviewStatus): Observable<ReviewDto[]> {
    return this.api.get<ReviewDto[]>(`${this.path}/status/${status}`);
  }

  getFlagged(): Observable<ReviewDto[]> {
    return this.api.get<ReviewDto[]>(`${this.path}/flagged`);
  }
}
