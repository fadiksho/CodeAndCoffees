import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { PushNotificationAddComponent } from "./push-notification-add.component";

describe("PushNotificationAddComponent", () => {
  let component: PushNotificationAddComponent;
  let fixture: ComponentFixture<PushNotificationAddComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [PushNotificationAddComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PushNotificationAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
