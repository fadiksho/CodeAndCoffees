import { async, ComponentFixture, TestBed } from "@angular/core/testing";

import { BlobListComponent } from "./blob-list.component";

describe("BlobListComponent", () => {
  let component: BlobListComponent;
  let fixture: ComponentFixture<BlobListComponent>;

  beforeEach(async(() => {
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
