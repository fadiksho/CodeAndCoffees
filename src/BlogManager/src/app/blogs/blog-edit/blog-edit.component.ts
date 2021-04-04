import {
  Component,
  OnInit,
  OnDestroy,
  ElementRef,
  ViewChildren,
  AfterViewInit
} from "@angular/core";
import {
  FormGroup,
  FormBuilder,
  Validators,
  FormArray,
  FormControlName
} from "@angular/forms";
import { DatePipe } from "@angular/common";
import { BlogDto } from "../_model/blogDto";
import { Observable, fromEvent, merge } from "rxjs";
import { ActivatedRoute, Router } from "@angular/router";
import { tap, debounceTime, map } from "rxjs/operators";
import { BlogService } from "../_services/blog.service";
import { Blog } from "../_model/blog";
import { GenericValidator } from "src/app/shared/_model/generic-validator";
import { ValidationModel } from "src/app/shared/_model/validation.model";
import { ToastrService } from "ngx-toastr";
import { HtmlService } from "../_services/html.service";
import { NgbNavChangeEvent } from "@ng-bootstrap/ng-bootstrap";

@Component({
  selector: "app-blog-edit",
  templateUrl: "./blog-edit.component.html",
  styleUrls: ["./blog-edit.component.scss"]
})
export class BlogEditComponent implements OnInit, OnDestroy, AfterViewInit {
  @ViewChildren(FormControlName, { read: ElementRef })
  formInputElements: ElementRef[];

  private genericValidator: GenericValidator;

  validationMessages: { [key: string]: ValidationModel };
  displayMessage: { [key: string]: string } = {};
  blogId: number;
  blogDtoForm: FormGroup;
  blogDto: BlogDto;
  blog: Blog;
  descriptionPreview: string;
  bodyPreview: string;
  loading = false;
  error: string;
  errorMessages: string;

  constructor(
    private fb: FormBuilder,
    public datepipe: DatePipe,
    private route: ActivatedRoute,
    private router: Router,
    private blogService: BlogService,
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

    this.route.params.subscribe(params => {
      this.blogId = +params["id"];
    });
    this.blogService
      .getBlog(this.blogId)
      .pipe(
        tap((blog: Blog) => {
          this.blog = blog;
        }),
        map(
          (blog: Blog) =>
            <BlogDto>{
              title: blog.title,
              description: blog.description,
              body: blog.body,
              tags: blog.tags,
              publishedDate: blog.publishedDate,
              isPublished: blog.isPublished
            }
        )
      )
      .subscribe((blogDto: BlogDto) => {
        this.blogDto = blogDto;
        this.onBlogDtoRetrived();
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
      .subscribe(() => {
        this.displayMessage = this.genericValidator.processMessages(
          this.blogDtoForm
        );
      });
  }
  onBlogDtoRetrived(): void {
    if (this.blogDtoForm) {
      this.blogDtoForm.reset();
    }
    // Update the data on the form
    this.blogDtoForm.patchValue({
      title: this.blogDto.title,
      description: this.htmlService.getHtmlValueForEdit(
        this.blogDto.description
      ),
      body: this.htmlService.getHtmlValueForEdit(this.blogDto.body),
      publishedDate: this.datepipe.transform(
        this.blogDto.publishedDate,
        "yyyy-MM-dd"
      ),
      isPublished: this.blogDto.isPublished
    });
    this.blogDto.tags.forEach(tag => {
      this.tags.push(this.fb.control(tag, Validators.required));
    });
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

  get tags(): FormArray {
    return this.blogDtoForm.get("tags") as FormArray;
  }
  addTag() {
    this.tags.push(this.fb.control("", Validators.required));
  }
  deleteTag(index: number): void {
    this.tags.removeAt(index);
  }
  updateBlog() {
    this.blogDto = Object.assign({}, this.blogDtoForm.value);
    // patch the generated html
    this.blogDto.description = this.htmlService.getHtmlValueForPreview(
      this.blogDtoForm.value["description"]
    );
    this.blogDto.body = this.htmlService.getHtmlValueForPreview(
      this.blogDtoForm.value["body"]
    );
    this.blogService.updateBlog(this.blogId, this.blogDto).subscribe(
      () => {
        this.toastr.success("Updated Successfully", "Update Status");
        this.router.navigate(["/blog"]);
      },
      (error: any) => {
        this.toastr.error(error.error + "Update Failed!", "Error");
      }
    );
  }
  deleteBlog(): void {
    if (this.blogId) {
      if (confirm("Confirm The Delete!")) {
        this.blogService.deleteBlog(this.blogId).subscribe(
          () => {
            this.toastr.info("Blog Deleted Successfuly", "Info");
            this.router.navigate(["/blog"]);
          },
          (errorMessage: any) => {
            this.toastr.error(
              "Blog Delete Failed! error message:" + errorMessage.error,
              "Error"
            );
          }
        );
      }
    }
  }
  ngOnDestroy(): void {}
}
