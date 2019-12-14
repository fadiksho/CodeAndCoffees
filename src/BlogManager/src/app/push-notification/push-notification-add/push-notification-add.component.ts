import {
  Component,
  OnInit,
  ViewChildren,
  ElementRef,
  AfterViewInit
} from "@angular/core";
import {
  FormControlName,
  FormGroup,
  FormBuilder,
  Validators
} from "@angular/forms";
import { GenericValidator } from "src/app/shared/_model/generic-validator";
import { ValidationModel } from "src/app/shared/_model/validation.model";
import { ToastrService } from "ngx-toastr";
import { merge, Observable, fromEvent } from "rxjs";
import { debounceTime } from "rxjs/operators";
import { PushNotificationPayload } from "../_model/push-notification-payload";
import { PushNotificationService } from "../_services/push-notification.service";

@Component({
  selector: "app-push-notification-add",
  templateUrl: "./push-notification-add.component.html",
  styleUrls: ["./push-notification-add.component.scss"]
})
export class PushNotificationAddComponent implements OnInit, AfterViewInit {
  @ViewChildren(FormControlName, { read: ElementRef })
  formInputElements: ElementRef[];

  private genericValidator: GenericValidator;

  validationMessages: { [key: string]: ValidationModel };
  displayMessage: { [key: string]: string } = {};
  pushNotificationForm: FormGroup;
  pushNotificationPayload: PushNotificationPayload;

  constructor(
    private fb: FormBuilder,
    private toastr: ToastrService,
    private pushNotificationService: PushNotificationService
  ) {
    this.validationMessages = {
      title: {
        validationMessages: {
          required: "Message Title Is Required"
        },
        childContollers: null
      },
      body: {
        validationMessages: {
          required: "Message Body Is Required"
        },
        childContollers: null
      }
    };
  }
  ngOnInit() {
    this.genericValidator = new GenericValidator(this.validationMessages);
    this.pushNotificationForm = this.fb.group({
      title: ["", [Validators.required]],
      body: ["", [Validators.required]],
      url: ["", []]
    });
  }

  ngAfterViewInit(): void {
    // Watch for the blur event from any input element on the form.
    const controlBlurs: Observable<any>[] = this.formInputElements.map(
      (formControl: ElementRef) => fromEvent(formControl.nativeElement, "blur")
    );

    // Merge the blur event observable with the valueChanges observable
    merge(this.pushNotificationForm.valueChanges, ...controlBlurs)
      .pipe(debounceTime(800))
      .subscribe(value => {
        this.displayMessage = this.genericValidator.processMessages(
          this.pushNotificationForm
        );
      });
  }

  pushIt() {
    this.pushNotificationPayload = Object.assign(
      {},
      this.pushNotificationForm.value
    );
    this.pushNotificationService
      .newPushNotification(this.pushNotificationPayload)
      .subscribe(
        res => {
          this.toastr.success(
            `Successed: ${res.messageSentCount},
         Faild: ${res.messageFaildCount},
         All: ${res.subscribersCount},`,
            "Notification"
          );
        },
        error => {
          this.toastr.error(
            "Push Notification Failed error message: " + error.error,
            "Error"
          );
        }
      );
  }
}
