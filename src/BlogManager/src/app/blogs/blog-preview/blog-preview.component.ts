import { Component, OnInit, Input } from "@angular/core";
import { Blog } from "../_model/blog";

@Component({
  selector: "app-blog-preview",
  templateUrl: "./blog-preview.component.html",
  styleUrls: ["./blog-preview.component.scss"]
})
export class BlogPreviewComponent implements OnInit {
  @Input() blog: Blog;

  constructor() {}

  ngOnInit() {}
}
