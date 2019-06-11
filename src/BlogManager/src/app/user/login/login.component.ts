import {
  Component,
  OnInit,
  ElementRef,
  ViewChildren,
  AfterViewInit
} from "@angular/core";
import {
  Validators,
  FormControl,
  FormGroup,
  FormBuilder,
  FormControlName
} from "@angular/forms";
import { AuthService } from "../auth.service";
import { Router, ActivatedRoute } from "@angular/router";
import { first, debounceTime } from "rxjs/operators";
import { GenericValidator } from "src/app/shared/_model/generic-validator";
import { Observable, pipe, fromEvent, observable, merge } from "rxjs";
import { ToastrService } from "ngx-toastr";
import { ValidationModel } from "src/app/shared/_model/validation.model";

@Component({
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: ["./login.component.scss"]
})
export class LoginComponent implements OnInit, AfterViewInit {
  @ViewChildren(FormControlName, { read: ElementRef })
  formInputElements: ElementRef[];
  displayMessage: { [key: string]: string } = {};
  validationMessages: { [key: string]: ValidationModel };
  private genericValidator: GenericValidator;

  loginForm: FormGroup;
  returnUrl: string;
  loading = false;
  error: string;
  errorMessages: string;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService
  ) {
    this.validationMessages = {
      userName: {
        validationMessages: {
          required: "User Name Is Required"
        },
        childContollers: null
      },
      password: {
        validationMessages: {
          required: "Password Is Required"
        },
        childContollers: null
      }
    };

    this.genericValidator = new GenericValidator(this.validationMessages);
  }

  ngOnInit() {
    this.loginForm = this.fb.group({
      userName: ["", Validators.required],
      password: ["", Validators.required]
    });

    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams["returnUrl"] || "/";
  }

  ngAfterViewInit(): void {
    // Watch for the blur event from any input element on the form.
    const controlBlurs: Observable<any>[] = this.formInputElements.map(
      (formControl: ElementRef) => fromEvent(formControl.nativeElement, "blur")
    );

    // Merge the blur event observable with the valueChanges observable
    merge(this.loginForm.valueChanges, ...controlBlurs)
      .pipe(debounceTime(800))
      .subscribe(value => {
        this.displayMessage = this.genericValidator.processMessages(
          this.loginForm
        );
      });
  }

  login() {
    if (this.loginForm.invalid) {
      return true;
    }
    this.loading = true;

    this.auth
      .login(this.loginForm.value.userName, this.loginForm.value.password)
      .pipe(first())
      .subscribe(
        user => {
          this.router.navigate([this.returnUrl]);
        },
        errorMessage => {
          this.errorMessages = errorMessage.error;
          this.toastr.error(errorMessage.error, "Error: " + errorMessage.name);
          this.loading = false;
          console.log(errorMessage);
        }
      );
  }
}
