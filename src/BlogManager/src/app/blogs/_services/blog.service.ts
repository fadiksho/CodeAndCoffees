import { Injectable } from "@angular/core";
import { BlogDto } from "../_model/blogDto";
import { Observable } from "rxjs";
import { HttpClient } from "@angular/common/http";
import { Blog } from "../_model/blog";
import { PaggingResult } from "../../shared/_model/paggingResult";
import { environment } from "src/environments/environment";
import { SharedService } from "../../shared/shared.service";
import { PaggingQuery } from "../../shared/_model/paggingQuery";

@Injectable()
export class BlogService {
  apiRoot = environment.apiRoot + "blog/";

  constructor(private http: HttpClient, private sharedService: SharedService) {}

  getBlog(blogId: number): Observable<Blog> {
    return this.http.get<Blog>(this.apiRoot + blogId);
  }

  getBlogPage(query: PaggingQuery): Observable<PaggingResult<Blog>> {
    return this.http.get<PaggingResult<Blog>>(
      this.apiRoot + "?" + this.sharedService.toQueryString(query)
    );
  }

  addBlog(newBlog: BlogDto): Observable<void> {
    return this.http.post<void>(this.apiRoot, newBlog);
  }
  updateBlog(blogId: number, updatedBlog: BlogDto): Observable<void> {
    return this.http.put<void>(this.apiRoot + blogId, updatedBlog);
  }
  deleteBlog(blogId: number): Observable<void> {
    return this.http.delete<void>(this.apiRoot + blogId);
  }
}
