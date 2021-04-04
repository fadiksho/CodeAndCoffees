import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { PaggingComponent } from "./pagging.component";

describe("PaggingComponent", () => {
  let component: PaggingComponent;
  let fixture: ComponentFixture<PaggingComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [PaggingComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PaggingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
