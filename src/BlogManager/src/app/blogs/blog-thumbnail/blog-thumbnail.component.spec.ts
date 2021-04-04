import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { BlogThumbnailComponent } from "./blog-thumbnail.component";

describe("BlogThumbnailComponent", () => {
  let component: BlogThumbnailComponent;
  let fixture: ComponentFixture<BlogThumbnailComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [BlogThumbnailComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BlogThumbnailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
