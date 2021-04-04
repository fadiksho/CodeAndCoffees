import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { BlobListComponent } from "./blob-list.component";

describe("BlobListComponent", () => {
  let component: BlobListComponent;
  let fixture: ComponentFixture<BlobListComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [BlobListComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BlobListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
