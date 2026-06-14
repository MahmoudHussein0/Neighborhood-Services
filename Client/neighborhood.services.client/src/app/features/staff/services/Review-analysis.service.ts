import { HttpClient } from "@angular/common/http";
import { environment } from "../../../environments/environment";

import { Injectable } from "@angular/core";
import { ReviewAnalysisDto } from "../models/ReviewAnalysis.model";

@Injectable({
  providedIn: 'root'
})

export class ReviewAnalysisService {

  private apiUrl = `${environment.apiUrl}/api/ReviewAnalysis`;

  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get<ReviewAnalysisDto[]>(this.apiUrl);
  }

  getById(id: number) {
    return this.http.get<ReviewAnalysisDto>(
      `${this.apiUrl}/${id}`
    );
  }
}