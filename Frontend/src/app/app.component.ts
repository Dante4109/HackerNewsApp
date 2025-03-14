import { Component, OnInit } from '@angular/core';
import { ApiService } from './api.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  stories: any[] = [];
  currentPage = 1;
  itemsPerPage = 10;
  searchQuery: string = '';

  constructor(private hackerNewsService: ApiService) {}
  title = 'Frontend';

  get paginatedStories() {
    return this.stories.slice(
      (this.currentPage - 1) * this.itemsPerPage,
      this.currentPage * this.itemsPerPage
    );
  }

  loadStories(): void {
    this.hackerNewsService.getNewestStories().subscribe((data: any) => {
      this.stories = data;
    });
  }

  onSearch(): void {
    /* this.stories = this.stories.filter((story) => {
      return story.title.includes(this.searchQuery);
    }); */

    this.hackerNewsService.getNewestStories().subscribe((data: any) => {
      this.stories = data.filter((story: { title: string | string[] }) => {
        return story.title.includes(this.searchQuery);
      });
    });

    console.log(this.stories);
  }

  ngOnInit() {
    this.loadStories();
  }
}
