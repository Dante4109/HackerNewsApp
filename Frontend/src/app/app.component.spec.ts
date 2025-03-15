import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { FormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { of } from 'rxjs';
import { ApiService } from './api.service';

class MockApiService {
  getNewestStories() {
    return of(
      new Array(100).fill(null).map((_, index) => ({
        title: `Story ${index + 1}`,
        url: (index + 1) % 10 === 0 ? '' : 'https://example.com',
        // Set URL to "" for every 10th item
      }))
    );
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

  it('should display the title on load', () => {
    fixture.detectChanges(); // Trigger initial change detection
    const titleElement = fixture.debugElement.query(By.css('.header-title')); // Adjust selector as needed
    expect(titleElement).toBeTruthy(); // Ensures the element exists
    expect(titleElement.nativeElement.offsetParent).not.toBeNull(); // Checks if it's visible (not `display: none`)
    expect(titleElement.nativeElement.textContent).toEqual('Hacker News'); // Checks if it's visible (not `display: none`)
  });

  it('should fetch newest stories from ApiService onInit', () => {
    spyOn(apiService, 'getNewestStories').and.callThrough();
    component.ngOnInit();
    expect(apiService.getNewestStories).toHaveBeenCalled();
    expect(component.stories.length).toBe(100);
  });

  it('should show loading message when isLoading is true', () => {
    component.isLoading = true;
    fixture.detectChanges();
    const loadingElement = fixture.debugElement.query(By.css('p'));
    expect(loadingElement.nativeElement.textContent).toEqual('Loading...');
  });

  it('should display an error message when errorMessage is set', () => {
    component.errorMessage = 'Error fetching data';
    fixture.detectChanges();
    const errorElement = fixture.debugElement.query(By.css('p'));
    expect(errorElement.nativeElement.textContent).toContain(
      'Error fetching data'
    );
  });

  it('should disable Previous button on first page after data has loaded', () => {
    component.currentPage = 1;
    fixture.detectChanges();
    const prevButton = fixture.debugElement.query(By.css('button.btnPrev'));
    const nextButton = fixture.debugElement.query(By.css('button.btnNext'));
    expect(prevButton.nativeElement.disabled).toBeTrue();
    expect(nextButton.nativeElement.disabled).toBeFalse();
  });

  it('should display the first 10 stories when data is available', () => {
    const storyElements = fixture.debugElement.queryAll(By.css('p'));
    const story1ElementText = storyElements[0].nativeElement.textContent;
    const story10ElementText = storyElements[9].nativeElement.textContent;
    expect(storyElements.length).toBeGreaterThan(0);
    expect(storyElements.length).toBeLessThan(11);
    expect(story1ElementText).toEqual(' Story 1 click here to view');
    expect(story10ElementText).toEqual(' Story 10 link not available');
  });

  it('should display the next 10 stories when next button is clicked', () => {
    const nextButton = fixture.debugElement.query(By.css('button.btnNext'));
    nextButton.triggerEventHandler('click', null);
    fixture.detectChanges();
    const storyElements = fixture.debugElement.queryAll(By.css('p'));
    const story1ElementText = storyElements[0].nativeElement.textContent;
    const story10ElementText = storyElements[9].nativeElement.textContent;
    expect(storyElements.length).toBeGreaterThan(0);
    expect(storyElements.length).toBeLessThan(11);
    expect(story1ElementText).toEqual(' Story 11 click here to view');
    expect(story10ElementText).toEqual(' Story 20 link not available');
  });

  it('should call onSearch when search button is clicked', () => {
    spyOn(component, 'onSearch');
    const button = fixture.debugElement.query(By.css('button.btnSearch'));
    button.triggerEventHandler('click', null);
    expect(component.onSearch).toHaveBeenCalled();
  });

  it('should enter "Story 30" into the search form and press the search button', () => {
    spyOn(component, 'onSearch');
    const input = fixture.debugElement.query(
      By.css('input.frmSearch')
    ).nativeElement;
    const button = fixture.debugElement.query(By.css('button.btnSearch'));
    input.value = 'Story 30';
    input.dispatchEvent(new Event('input'));
    button.triggerEventHandler('click', null);
    fixture.detectChanges();
    expect(component.onSearch).toHaveBeenCalled();
    expect(component.searchQuery).toBe('Story 30');
  });

  it(`should display only Story 25 when "Story 25" is typed in the search form 
    and the search button is pressed`, () => {
    spyOn(component, 'onSearch').and.callThrough();
    const input = fixture.debugElement.query(
      By.css('input.frmSearch')
    ).nativeElement;
    input.value = 'Story 25';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();
    const searchButton = fixture.debugElement.query(By.css('button.btnSearch'));
    searchButton.triggerEventHandler('click', null);
    fixture.detectChanges();
    const storyElements = fixture.debugElement.queryAll(By.css('p'));
    const story1ElementText = storyElements[0].nativeElement.textContent;
    expect(storyElements.length).toBeGreaterThan(0);
    expect(storyElements.length).toBeLessThan(2);
    expect(story1ElementText).toEqual(' Story 25 click here to view');
  });

  it('should disable Previous button on first page', () => {
    const prevButton = fixture.debugElement.query(By.css('button.btnPrev'));
    const nextButton = fixture.debugElement.query(By.css('button.btnNext'));
    expect(prevButton.nativeElement.disabled).toBeTrue();
    expect(nextButton.nativeElement.disabled).toBeFalse();
  });

  it('should disable Next button when on the last page', () => {
    component.currentPage = 10;
    fixture.detectChanges();
    const prevButton = fixture.debugElement.query(By.css('button.btnPrev'));
    const nextButton = fixture.debugElement.query(By.css('button.btnNext'));
    expect(prevButton.nativeElement.disabled).toBeFalse();
    expect(nextButton.nativeElement.disabled).toBeTrue();
  });
});
