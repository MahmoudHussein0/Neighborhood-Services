import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
@Injectable({
  providedIn: 'root'
})
export class SupportTicketsService {
filters = {
  status: '',
  priority: '',
  search: '',
  page: 1,
  pageSize: 10
};
private readonly baseUrl = `${environment.apiUrl}/api/SupportTickets`;
  constructor(private http: HttpClient) {}

 
getTickets(filters: any) {
  let params = new HttpParams();

  Object.keys(filters).forEach(key => {
    if (filters[key] !== null && filters[key] !== '') {
      params = params.set(key, filters[key]);
    }
  });

  return this.http.get(this.baseUrl, { params });
}
  getTicketDetails(id: number) {
    return this.http.get(`${this.baseUrl}/${id}/details`);
  }
// دالة تحديث الحالة -> /api/SupportTickets/{id}/status
updateTicketStatus(id: number, status: number) {
  // بنبعت رقم الـ Enum مباشرة في الـ Body زي ما الـ [FromBody] مستني في الباك إند
  return this.http.put(`${this.baseUrl}/${id}/status`, status, {
    headers: { 'Content-Type': 'application/json' } // تأكيد إن الداتا رايحة كـ JSON صريح
  });
}

// دالة تحديث الأولوية -> /api/SupportTickets/{id}/priority
updateTicketPriority(id: number, priority: number) {
  return this.http.put(`${this.baseUrl}/${id}/priority`, priority, {
    headers: { 'Content-Type': 'application/json' }
  });
}
// تعديل دالة الـ Update لتستقبل وتُرسل الكائن بالكامل 🎯
updateTicket(id: number, ticketData: { status: number; priority: number }) {
  return this.http.put(`${this.baseUrl}/${id}`, ticketData);
}
deleteTicket(id: number) {
  return this.http.delete(`${this.baseUrl}/${id}`);
}
}