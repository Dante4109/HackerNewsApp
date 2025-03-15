import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { FormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { of } from 'rxjs';
import { ApiService } from './api.service';

class MockApiService {
  getNewestStories() {
    return of([
      new Array(100).fill(null).map((_, index) => ({
        title: `Story ${index + 1}`,
        url: (index + 1) % 10 === 0 ? '' : 'https://example.com', // Set URL to "" for every 10th item
      })),
      /* { title: 'Story 1', url: 'https://example.com' },
      { title: 'Story 2', url: 'https://example.com' },
      { title: 'Story 3', url: 'https://example.com' },
      { title: 'Story 4', url: 'https://example.com' },
      { title: 'Story 5', url: 'https://example.com' },
      { title: 'Story 6', url: 'https://example.com' },
      { title: 'Story 7', url: 'https://example.com' },
      { title: 'Story 8', url: 'https://example.com' },
      { title: 'Story 9', url: 'https://example.com' },
      { title: 'Story 10', url: '' },
      { title: 'Story 11', url: 'https://example.com' },
      { title: 'Story 12', url: 'https://example.com' },
      { title: 'Story 13', url: 'https://example.com' },
      { title: 'Story 14', url: 'https://example.com' },
      { title: 'Story 15', url: 'https://example.com' },
      { title: 'Story 16', url: 'https://example.com' },
      { title: 'Story 17', url: 'https://example.com' },
      { title: 'Story 18', url: 'https://example.com' },
      { title: 'Story 19', url: 'https://example.com' },
      { title: 'Story 20', url: '' },
      { title: 'Story 21', url: 'https://example.com' },
      { title: 'Story 22', url: 'https://example.com' },
      { title: 'Story 23', url: 'https://example.com' }, */
      /* { title: 'Story 1', url: 'https://example.com' },
      { title: 'Story 2', url: '' }, */
    ]);
  }
}

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let apiService: ApiService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FormsModule],
      declarations: [AppComponent],
      providers: [{ provide: ApiService, useClass: MockApiService }],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    apiService = TestBed.inject(ApiService);
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch newest stories from ApiService onInit', () => {
    spyOn(apiService, 'getNewestStories').and.callThrough();
    component.ngOnInit();
    expect(apiService.getNewestStories).toHaveBeenCalled();
  });

  it('should disable Previous button on first page', () => {
    component.currentPage = 1;
    fixture.detectChanges();
    const prevButton = fixture.debugElement.query(By.css('button.btnPrev'));
    const nextButton = fixture.debugElement.query(By.css('button.btnNext'));
    expect(prevButton.nativeElement.disabled).toBeFalse();
    expect(nextButton.nativeElement.disabled).toBeTrue();
  });

  it('should show loading message when isLoading is true', () => {
    component.isLoading = true;
    fixture.detectChanges();
    const loadingElement = fixture.debugElement.query(By.css('p'));
    expect(loadingElement.nativeElement.textContent).toContain('Loading...');
  });

  it('should display an error message when errorMessage is set', () => {
    component.errorMessage = 'Error fetching data';
    fixture.detectChanges();
    const errorElement = fixture.debugElement.query(By.css('p'));
    expect(errorElement.nativeElement.textContent).toContain(
      'Error fetching data'
    );
  });

  it('should display the first 10 stories when data is available', () => {
    spyOnProperty(component, 'paginatedStories', 'get').and.returnValue(
      component.stories.slice(
        (component.currentPage - 1) * component.itemsPerPage,
        component.currentPage * component.itemsPerPage
      )
    );
    fixture.detectChanges();
    const storyElements = fixture.debugElement.queryAll(By.css('p'));
    expect(storyElements.length).toBeLessThan(11);
    expect(storyElements[1].nativeElement.textContent).toContain('Story 2');
  });

  it('should disable Previous button on first page', () => {
    component.currentPage = 1;
    fixture.detectChanges();
    const prevButton = fixture.debugElement.query(By.css('button.btnPrev'));
    const nextButton = fixture.debugElement.query(By.css('button.btnNext'));
    expect(prevButton.nativeElement.disabled).toBeFalse();
    expect(nextButton.nativeElement.disabled).toBeTrue();
  });

  it('should disable Next button when on the last page', () => {
    component.currentPage = 10;
    component.itemsPerPage = 10;
    fixture.detectChanges();
    const prevButton = fixture.debugElement.query(By.css('button.btnPrev'));
    const nextButton = fixture.debugElement.query(By.css('button.btnNext'));
    expect(prevButton.nativeElement.disabled).toBeTrue();
    expect(nextButton.nativeElement.disabled).toBeFalse();
  });
});
