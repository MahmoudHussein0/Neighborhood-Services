import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  ReviewDto,
  CreateReviewRequest,
  UpdateReviewRequest,
  ReviewStatus,
} 
from '../models/Review.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ReviewsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/Reviews`;

  getAll(): Observable<ReviewDto[]> {
    return this.http.get<ReviewDto[]>(this.base);
  }

  getById(id: number): Observable<ReviewDto> {
    return this.http.get<ReviewDto>(`${this.base}/${id}`);
  }

  create(payload: CreateReviewRequest): Observable<ReviewDto> {
    return this.http.post<ReviewDto>(this.base, payload);
  }

  update(id: number, payload: UpdateReviewRequest): Observable<ReviewDto> {
    return this.http.put<ReviewDto>(`${this.base}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  getByReviewer(reviewerId: string): Observable<ReviewDto[]> {
    return this.http.get<ReviewDto[]>(`${this.base}/reviewer/${reviewerId}`);
  }

  getByReviewee(revieweeId: string): Observable<ReviewDto[]> {
    return this.http.get<ReviewDto[]>(`${this.base}/reviewee/${revieweeId}`);
  }

  getByStatus(status: ReviewStatus): Observable<ReviewDto[]> {
    return this.http.get<ReviewDto[]>(`${this.base}/status/${status}`);
  }

  getFlagged(): Observable<ReviewDto[]> {
    return this.http.get<ReviewDto[]>(`${this.base}/flagged`);
  }
}