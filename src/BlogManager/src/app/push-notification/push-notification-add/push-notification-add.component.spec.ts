import { async, ComponentFixture, TestBed } from "@angular/core/testing";

import { PushNotificationAddComponent } from "./push-notification-add.component";

describe("PushNotificationAddComponent", () => {
  let component: PushNotificationAddComponent;
  let fixture: ComponentFixture<PushNotificationAddComponent>;

  beforeEach(async(() => {
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
