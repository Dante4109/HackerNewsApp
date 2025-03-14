import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private apiUrl = 'http://localhost:5292/api/hackernews/';

  constructor(private http: HttpClient) {}

  getNewestStories(): Observable<any> {
    return this.http.get(this.apiUrl);
  }
}
