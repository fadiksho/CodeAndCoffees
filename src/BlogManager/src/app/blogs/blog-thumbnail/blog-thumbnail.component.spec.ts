import { async, ComponentFixture, TestBed } from "@angular/core/testing";

import { BlogThumbnailComponent } from "./blog-thumbnail.component";

describe("BlogThumbnailComponent", () => {
  let component: BlogThumbnailComponent;
  let fixture: ComponentFixture<BlogThumbnailComponent>;

  beforeEach(async(() => {
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
