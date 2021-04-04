import {
  Component,
  OnInit,
  AfterViewInit,
  ElementRef,
  ViewChildren
} from "@angular/core";
import {
  FormGroup,
  FormBuilder,
  Validators,
  FormArray,
  FormControl,
  FormControlName
} from "@angular/forms";
import { DatePipe } from "@angular/common";

import { Observable, fromEvent, merge } from "rxjs";
import { debounceTime } from "rxjs/operators";
import { BlogService } from "../_services/blog.service";
import { Router } from "@angular/router";

import { ToastrService } from "ngx-toastr";
import { BlogDto } from "../_model/blogDto";
import { ValidationModel } from "src/app/shared/_model/validation.model";
import { GenericValidator } from "src/app/shared/_model/generic-validator";
import { HtmlService } from "../_services/html.service";
import { NgbNavChangeEvent } from "@ng-bootstrap/ng-bootstrap";

@Component({
  selector: "app-blog-add",
  templateUrl: "./blog-add.component.html",
  styleUrls: ["./blog-add.component.scss"]
})
export class BlogAddComponent implements OnInit, AfterViewInit {
  @ViewChildren(FormControlName, { read: ElementRef })
  formInputElements: ElementRef[];

  private genericValidator: GenericValidator;

  validationMessages: { [key: string]: ValidationModel };
  displayMessage: { [key: string]: string } = {};
  blogDtoForm: FormGroup;
  blogDto: BlogDto;
  descriptionPreview: string;
  bodyPreview: string;

  constructor(
    private fb: FormBuilder,
    public datepipe: DatePipe,
    private blogService: BlogService,
    private router: Router,
    private toastr: ToastrService,
    private htmlService: HtmlService
  ) {
    this.validationMessages = {
      title: {
        validationMessages: {
          required: "Title Is Required"
        },
        childContollers: null
      },
      description: {
        validationMessages: {
          required: "Description Is Required"
        },
        childContollers: null
      },
      body: {
        validationMessages: {
          required: "Body Is Required"
        },
        childContollers: null
      },
      publishedDate: {
        validationMessages: {
          required: "Publish Date Is Required"
        },
        childContollers: null
      },
      tags: {
        validationMessages: {},
        childContollers: {
          tag: {
            validationMessages: {
              required: "Tag Shouldn't Be Emplty"
            },
            childContollers: null
          }
        }
      }
    };
  }

  ngOnInit() {
    this.genericValidator = new GenericValidator(this.validationMessages);
    this.blogDtoForm = this.fb.group({
      title: ["", [Validators.required]],
      description: ["", [Validators.required]],
      body: ["", [Validators.required]],
      tags: this.fb.array([]),
      publishedDate: [
        this.datepipe.transform(Date.now(), "yyyy-MM-dd"),
        [Validators.required]
      ],
      isPublished: false
    });
  }
  ngAfterViewInit(): void {
    // Watch for the blur event from any input element on the form.
    const controlBlurs: Observable<any>[] = this.formInputElements.map(
      (formControl: ElementRef) => fromEvent(formControl.nativeElement, "blur")
    );

    // Merge the blur event observable with the valueChanges observable
    merge(this.blogDtoForm.valueChanges, ...controlBlurs)
      .pipe(debounceTime(800))
      .subscribe(value => {
        this.displayMessage = this.genericValidator.processMessages(
          this.blogDtoForm
        );
      });
  }
  get tags(): FormArray {
    return <FormArray>this.blogDtoForm.get("tags");
  }
  addTag() {
    this.tags.push(new FormControl("", Validators.required));
  }
  deleteTag(index: number): void {
    this.tags.removeAt(index);
  }
  addBlog() {
    this.blogDto = Object.assign({}, this.blogDtoForm.value);
    this.blogDto.description = this.htmlService.getHtmlValueForPreview(
      this.blogDtoForm.value["description"]
    );
    this.blogDto.body = this.htmlService.getHtmlValueForPreview(
      this.blogDtoForm.value["body"]
    );
    this.blogService.addBlog(this.blogDto).subscribe(
      (rse: void) => {
        this.toastr.success("Blog Created Succesfully", "Notification");
      },
      (error: any) => {
        this.toastr.error(
          "Creat Blog Fail error message: " + error.error,
          "Error"
        );
      }
    );
  }
  cancel() {
    if (confirm("Are you sure?")) {
      this.router.navigate(["/blog"]);
    }
  }

  onTapChange(event: NgbNavChangeEvent) {
    switch (event.nextId) {
      case "description-preview":
        this.descriptionPreview = this.htmlService.getHtmlValueForPreview(
          this.blogDtoForm.value["description"]
        );
        break;
      case "body-preview":
        this.bodyPreview = this.htmlService.getHtmlValueForPreview(
          this.blogDtoForm.value["body"]
        );
    }
  }
}
