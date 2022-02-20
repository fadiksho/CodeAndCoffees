import { HttpErrorResponse } from '@angular/common/http';
import {
  AfterViewInit,
  Component,
  ElementRef,
  OnInit,
  ViewChildren,
} from '@angular/core';
import {
  FormBuilder,
  FormControlName,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { debounceTime, fromEvent, merge, Observable } from 'rxjs';
import { GenericValidator } from 'src/app/shared/_model/generic-validator';
import { ValidationModel } from 'src/app/shared/_model/validation.model';
import { UserService } from 'src/app/_auth/user.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit, AfterViewInit {
  @ViewChildren(FormControlName, { read: ElementRef })
  formInputElements!: ElementRef[];

  private genericValidator!: GenericValidator;

  validationMessages: { [key: string]: ValidationModel };
  displayMessage: { [key: string]: string } = {};
  loginForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private toastr: ToastrService,
    private userService: UserService
  ) {
    this.validationMessages = {
      userName: {
        validationMessages: {
          required: 'Username Is Required',
        },
        childContollers: {},
      },
      password: {
        validationMessages: {
          required: 'Password Is Required',
        },
        childContollers: {},
      },
    };
  }

  ngOnInit() {
    this.genericValidator = new GenericValidator(this.validationMessages);
    this.loginForm = this.fb.group({
      userName: ['', [Validators.required]],
      password: ['', [Validators.required]],
      rememberMe: [false],
    });
  }

  ngAfterViewInit(): void {
    // Watch for the blur event from any input element on the form.
    const controlBlurs: Observable<any>[] = this.formInputElements.map(
      (formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur')
    );

    // Merge the blur event observable with the valueChanges observable
    merge(this.loginForm.valueChanges, ...controlBlurs)
      .pipe(debounceTime(800))
      .subscribe((value) => {
        this.displayMessage = this.genericValidator.processMessages(
          this.loginForm
        );
      });
  }

  login() {
    this.userService.login(this.loginForm.value).subscribe({
      error: (error: HttpErrorResponse) => {
        console.log(error);
        for (const key in error.error) {
          this.toastr.error(key + error.error[key], 'Error');
        }
      },
    });
  }
}
