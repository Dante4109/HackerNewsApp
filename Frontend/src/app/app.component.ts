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

  constructor(private hackerNewsService: ApiService) {}
  title = 'Frontend';

  ngOnInit() {
    this.hackerNewsService.getNewestStories().subscribe((data: any) => {
      this.stories = data;
    });
  }
}
