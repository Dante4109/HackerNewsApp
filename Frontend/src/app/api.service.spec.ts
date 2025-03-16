import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { ApiService } from './api.service';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { Story } from './story';
import { environment } from '../environments/environment';

describe('ApiService', () => {
  let service: ApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ApiService, provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(ApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should make a GET request to fetch newest stories', () => {
    const mockStories: Story[] = new Array(100).fill(null).map((_, index) => ({
      id: index + 1,
      title: `Story ${index + 1}`,
      url: (index + 1) % 10 === 0 ? '' : `https://story ${index + 1}.com`,
      // Set URL to "" for every 10th item
    }));

    service.getNewestStories().subscribe((stories) => {
      expect(stories.length).toBe(100);
      expect(stories).toEqual(mockStories);
    });

    const req = httpMock.expectOne(environment.apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush(mockStories);
  });

  it('should handle errors and return an error message', () => {
    service.getNewestStories().subscribe({
      next: () => fail('Expected an error, but got a response'),
      error: (error) => {
        expect(error.message).toBe(
          'Failed to fetch newest stories. Please try again later.'
        );
      },
    });

    const req = httpMock.expectOne(environment.apiUrl);
    req.flush('Something went wrong', {
      status: 500,
      statusText: 'Internal Server Error',
    });
  });
});
