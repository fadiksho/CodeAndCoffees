import { Component, OnInit, OnDestroy } from "@angular/core";
import { BlogService } from "../_services/blog.service";
import { PaggingResult } from "../../shared/_model/paggingResult";
import { Blog } from "../_model/blog";
import { ToastrService } from "ngx-toastr";
import { HttpErrorResponse } from "@angular/common/http";
import { BlogQuery } from "../_model/blogQuery";
@Component({
  selector: "app-blog-list",
  templateUrl: "./blog-list.component.html",
  styleUrls: ["./blog-list.component.scss"]
})
export class BlogListComponent implements OnInit {
  query: BlogQuery = {
    page: 1,
    pageSize: 10,
    tags: [],
    onlyPublished: true
  };

  paggingResult: PaggingResult<Blog>;

  constructor(
    private blogService: BlogService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.getBlogPage();
  }
  onPageChange(page: number) {
    this.query.page = page;
    this.getBlogPage();
  }
  onGetOnlyPublishedChange(onlyPublishedBlog: boolean) {
    this.query.onlyPublished = onlyPublishedBlog;
    this.getBlogPage();
  }
  private getBlogPage(): void {
    this.blogService.getBlogPage(this.query).subscribe(
      result => {
        this.paggingResult = result;
      },
      (error: HttpErrorResponse) => {
        this.toastr.error(error.error, "Error");
      }
    );
  }
}
