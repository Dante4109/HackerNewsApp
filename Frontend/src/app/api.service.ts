import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { environment } from '../environments/environment';
import { Story } from './story';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private apiUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getNewestStories(): Observable<Story[]> {
    return this.http.get<Story[]>(this.apiUrl).pipe(
      catchError((error) => {
        console.error('Error fetching newest stories:', error);
        return throwError(
          () =>
            new Error('Failed to fetch newest stories. Please try again later.')
        );
      })
    );
  }
}
