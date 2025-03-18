import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { FormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { delay, of } from 'rxjs';
import { ApiService } from './api.service';

class MockApiService {
  getNewestStories() {
    return of(
      new Array(100).fill(null).map((_, index) => ({
        id: index + 1,
        title: `Story ${index + 1}`,
        url: (index + 1) % 10 === 0 ? '' : `https://story ${index + 1}.com`,
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

  it('should display the title "Hacker News" on load', () => {
    fixture.detectChanges();
    const titleElement = fixture.debugElement.query(By.css('.header-title'));
    expect(titleElement).toBeTruthy();
    expect(titleElement.nativeElement.offsetParent).not.toBeNull();
    expect(titleElement.nativeElement.textContent).toEqual('Hacker News');
  });

  it('should fetch newest stories from ApiService onInit', () => {
    spyOn(apiService, 'getNewestStories').and.callThrough();
    component.ngOnInit();
    expect(apiService.getNewestStories).toHaveBeenCalled();
    expect(component.stories.length).toBe(100);
  });

  it('should show a loading message when async pipe has not yet loaded data', () => {
    spyOn(apiService, 'getNewestStories').and.returnValue(
      of([]).pipe(delay(1000))
    ); // Simulate delay
    component.ngOnInit();
    fixture.detectChanges();
    const loadingElement = fixture.debugElement.query(By.css('p'));
    expect(loadingElement.nativeElement.textContent).toEqual('Loading...');
  });

  it('should display an error message when the API fails to fetch data', () => {
    spyOn(apiService, 'getNewestStories').and.throwError(
      'Failed to fetch newest stories. Please try again later.'
    );
    component.ngOnInit();
    fixture.detectChanges();
    const errorElement = fixture.debugElement.query(By.css('p'));
    expect(errorElement.nativeElement.textContent).toContain(
      'Failed to fetch newest stories. Please try again later.'
    );
  });

  it('should disable Previous button on first page after data has loaded', () => {
    component.currentPage = 1;
    fixture.detectChanges();
    const prevButton = fixture.debugElement.query(
      By.css('button.button-previous')
    );
    const nextButton = fixture.debugElement.query(By.css('button.button-next'));
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
    const nextButton = fixture.debugElement.query(By.css('button.button-next'));
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
    const button = fixture.debugElement.query(By.css('button.button-search'));
    button.triggerEventHandler('click', null);
    expect(component.onSearch).toHaveBeenCalled();
  });

  it('should enter "Story 30" into the search form and press the search button', () => {
    spyOn(component, 'onSearch');
    const input = fixture.debugElement.query(
      By.css('input.form-search')
    ).nativeElement;
    const button = fixture.debugElement.query(By.css('button.button-search'));
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
      By.css('input.form-search')
    ).nativeElement;
    input.value = 'Story 25';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();
    const searchButton = fixture.debugElement.query(
      By.css('button.button-search')
    );
    searchButton.triggerEventHandler('click', null);
    fixture.detectChanges();
    const storyElements = fixture.debugElement.queryAll(By.css('p'));
    const story1ElementText = storyElements[0].nativeElement.textContent;
    expect(storyElements.length).toBeGreaterThan(0);
    expect(storyElements.length).toBeLessThan(2);
    expect(story1ElementText).toEqual(' Story 25 click here to view');
  });

  it(`should display "No stories found" when the search data in the search form
    does not match the title of any story that has beed loaded into memory`, () => {
    spyOn(component, 'onSearch').and.callThrough();
    const input = fixture.debugElement.query(
      By.css('input.form-search')
    ).nativeElement;
    input.value = 'asfasgadrdhadh';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();
    const searchButton = fixture.debugElement.query(
      By.css('button.button-search')
    );
    searchButton.triggerEventHandler('click', null);
    fixture.detectChanges();
    const elements = fixture.debugElement.queryAll(By.css('p'));
    const elementText = elements[0].nativeElement.textContent;
    expect(elements.length).toBeGreaterThan(0);
    expect(elements.length).toBeLessThan(2);
    expect(elementText).toEqual('No stories found');
  });

  it('should disable Previous button on first page', () => {
    const prevButton = fixture.debugElement.query(
      By.css('button.button-previous')
    );
    const nextButton = fixture.debugElement.query(By.css('button.button-next'));
    expect(prevButton.nativeElement.disabled).toBeTrue();
    expect(nextButton.nativeElement.disabled).toBeFalse();
  });

  it('should disable Next button when on the last page', () => {
    component.currentPage = 10;
    fixture.detectChanges();
    const prevButton = fixture.debugElement.query(
      By.css('button.button-previous')
    );
    const nextButton = fixture.debugElement.query(By.css('button.button-next'));
    expect(prevButton.nativeElement.disabled).toBeFalse();
    expect(nextButton.nativeElement.disabled).toBeTrue();
  });
});
