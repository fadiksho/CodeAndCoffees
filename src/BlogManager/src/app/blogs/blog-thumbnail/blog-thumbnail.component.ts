import { Component, OnInit, Input, ViewEncapsulation } from "@angular/core";
import { Blog } from "../_model/blog";

@Component({
  selector: "app-blog-thumbnail",
  templateUrl: "./blog-thumbnail.component.html",
  styleUrls: ["./blog-thumbnail.component.scss"],
  encapsulation: ViewEncapsulation.None
})
export class BlogThumbnailComponent implements OnInit {
  @Input() blog: Blog;

  constructor() {}

  ngOnInit() {}
}
