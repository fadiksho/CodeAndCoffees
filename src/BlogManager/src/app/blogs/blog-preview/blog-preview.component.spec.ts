import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { BlogPreviewComponent } from "./blog-preview.component";

describe("BlogPreviewComponent", () => {
  let component: BlogPreviewComponent;
  let fixture: ComponentFixture<BlogPreviewComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [BlogPreviewComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BlogPreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
