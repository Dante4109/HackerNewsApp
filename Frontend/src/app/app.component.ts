import { Component, OnInit } from '@angular/core';
import { ApiService } from './api.service';
import { Story } from './story';
import {
  catchError,
  debounceTime,
  distinctUntilChanged,
  tap,
} from 'rxjs/operators';
import { FormControl } from '@angular/forms';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  allStories$: Observable<Story[]> = of([]); // Store all stories once loaded
  stories: Story[] = []; // Displayed stories (filtered results)
  currentPage: number = 1;
  itemsPerPage: number = 10;
  errorMessage: string = '';
  searchQuery: string = '';
  searchControl = new FormControl('');

  title = 'Frontend';
  constructor(private hackerNewsService: ApiService) {}

  get paginatedStories(): Story[] {
    return this.slicedStories(this.stories);
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.stories.length / this.itemsPerPage));
  }

  slicedStories(stories: Story[]): Story[] {
    return stories.slice(
      (this.currentPage - 1) * this.itemsPerPage,
      this.currentPage * this.itemsPerPage
    );
  }

  loadStories(): void {
    this.allStories$ = this.hackerNewsService.getNewestStories().pipe(
      tap((data: Story[]) => {
        this.stories = [...data]; // Assign data to stories
      }),
      catchError((error) => {
        this.errorMessage = error.message;
        return of([]); // Return an empty array to keep the app stable
      })
    );
  }

  onSearch(): void {
    if (!this.searchQuery.trim()) {
      this.allStories$.subscribe((stories) => (this.stories = [...stories]));
    } else {
      this.allStories$.subscribe((stories) => {
        this.stories = stories.filter((story: Story) =>
          story.title.toLowerCase().includes(this.searchQuery.toLowerCase())
        );
      });
    }
    this.currentPage = 1; // Reset pagination
  }

  ngOnInit(): void {
    this.loadStories();

    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((query) => {
        this.searchQuery = query || '';
        this.onSearch();
      });
  }
}
