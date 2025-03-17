import { Component, OnInit } from '@angular/core';
import { ApiService } from './api.service';
import { Story } from './story';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  allStories: Story[] = []; // Store all stories once loaded
  stories: Story[] = []; // Displayed stories (filtered results)
  currentPage: number = 1;
  itemsPerPage: number = 10;
  isLoading: boolean = true;
  errorMessage: string = '';
  searchQuery: string = '';

  title = 'Frontend';
  constructor(private hackerNewsService: ApiService) {}

  get paginatedStories(): Story[] {
    return this.slicedStories(this.stories);
  }

  slicedStories(stories: Story[]): Story[] {
    return stories.slice(
      (this.currentPage - 1) * this.itemsPerPage,
      this.currentPage * this.itemsPerPage
    );
  }

  loadStories(): void {
    this.hackerNewsService.getNewestStories().subscribe({
      next: (data: Story[]) => {
        this.allStories = data; // Store all stories
        this.stories = [...this.allStories]; // Initialize displayed stories
        this.isLoading = false;
      },
      error: (error: any) => {
        this.errorMessage = error.message;
        this.isLoading = false;
      },
    });
  }

  onSearch(): void {
    if (!this.searchQuery.trim()) {
      // If search query is empty, show all stories again
      this.stories = [...this.allStories];
    } else {
      // Filter locally from allStories
      this.stories = this.allStories.filter((story: Story) =>
        story.title.toLowerCase().includes(this.searchQuery.toLowerCase())
      );
    }
    this.currentPage = 1; // Reset to first page after searching
  }

  ngOnInit(): void {
    this.loadStories();
  }
}
