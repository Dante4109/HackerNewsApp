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
  stories: Story[] = [];
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
      next: (data: any) => {
        this.stories = data;
        this.isLoading = false;
      },
      error: (error: any) => {
        this.errorMessage = 'Error fetching data.';
        this.isLoading = false;
      },
    });
  }

  onSearch(): void {
    this.hackerNewsService.getNewestStories().subscribe((data: any) => {
      this.stories = data.filter((story: { title: string | string[] }) => {
        return story.title.includes(this.searchQuery);
      });
    });
  }

  ngOnInit(): void {
    this.loadStories();
  }
}
