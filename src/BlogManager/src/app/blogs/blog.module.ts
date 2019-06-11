import { NgModule } from "@angular/core";
import { BlogListComponent } from "./blog-list/blog-list.component";
import { BlogEditComponent } from "./blog-edit/blog-edit.component";
import { BlogAddComponent } from "./blog-add/blog-add.component";
import { BlogThumbnailComponent } from "./blog-thumbnail/blog-thumbnail.component";
import { Routes, RouterModule } from "@angular/router";
import { SharedModule } from "../shared/shared.module";
import { BlogPreviewComponent } from "./blog-preview/blog-preview.component";
import { FilterComponent } from "./filter/filter.component";
import { AutosizeModule } from "ngx-autosize";
import { BlogService } from "./_services/blog.service";
import { HtmlService } from "./_services/html.service";
import { SafeHtmlPipe } from "./_pipes/safe-html.pipe";

const blogRoutes: Routes = [
  { path: "", component: BlogListComponent },
  { path: "edit/:id", component: BlogEditComponent },
  { path: "add", component: BlogAddComponent }
];

@NgModule({
  imports: [SharedModule, RouterModule.forChild(blogRoutes), AutosizeModule],
  declarations: [
    BlogListComponent,
    BlogEditComponent,
    BlogAddComponent,
    BlogThumbnailComponent,
    BlogPreviewComponent,
    FilterComponent,
    SafeHtmlPipe
  ],
  providers: [BlogService, HtmlService]
})
export class BlogModule {}
