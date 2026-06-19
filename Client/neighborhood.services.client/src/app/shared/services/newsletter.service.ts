import { Injectable } from '@angular/core';
import { ApiService } from '../../core/services/api.service';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';




@Injectable({
  providedIn: 'root',
})
export class NewsletterService {
private Endpoint = '/Newsletter';
constructor(private apiService: ApiService,private http: HttpClient) { }
newssubscribers?: string[];
objectTosend={
  "email":" "
}
//to get all subscribers
  getAll(): Observable<string[]> {
      return this.apiService.get<string[]>(this.Endpoint);
    }

  //to add a new subscriber
   subscribe(email:any): Observable<any> {
      return this.apiService.post(this.Endpoint,email);
    }

subscribbe(email: string) {
  return this.http.post(
    'https://localhost:7228/api/Newsletter',
    JSON.stringify(email),
    {
      headers: {
        'Content-Type': 'application/json'
      }
    }
  );
}

subscribbel(email: string) {
  return this.http.post(
    `${environment.apiUrl}${this.Endpoint}`,
    JSON.stringify(email),
    {
      headers: {
        'Content-Type': 'application/json'
      }
    }
  );
}



  //to publish a new letter
   publish(obj:any): Observable<any> {
      return this.apiService.post(this.Endpoint+'/SendNewsLetter',obj);
    }




}



