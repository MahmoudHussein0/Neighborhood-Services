import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LangService {

  private langSubject = new BehaviorSubject<string>('en');
  lang$ = this.langSubject.asObservable();

  setLanguage(lang: string) {
    this.langSubject.next(lang);
  }
}
