import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  catchError,
  Observable,
  throwError,
  shareReplay,
  tap,
  map,
} from 'rxjs';
import { environment } from '../environments/environment';
import { Story } from './story';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private apiUrl: string = environment.apiUrl;
  private storiesCache$: Observable<Story[]> | null = null;

  constructor(private http: HttpClient) {}

  getNewestStories(): Observable<Story[]> {
    if (!this.storiesCache$) {
      this.storiesCache$ = this.http
        .get<Story[]>(this.apiUrl, {
          observe: 'response', // Ensures we can check if a redirect happened
        })
        .pipe(
          tap((response) => {
            if (response.status === 307 || response.status === 302) {
              console.warn('Redirect detected:', response);
            }
          }),
          map((response) => response.body ?? []), // Extract the body and provide a fallback for null
          catchError((error) => {
            console.error('Error fetching newest stories:', error);
            return throwError(
              () =>
                new Error(
                  'Failed to fetch newest stories. Please try again later.'
                )
            );
          }),
          shareReplay(1) // Ensures the request is cached across subscriptions
        );
    }
    return this.storiesCache$;
  }
}
