import { Injectable } from "@angular/core";
import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpErrorResponse
} from "@angular/common/http";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { AuthService } from "../user/auth.service";
import { Router } from "@angular/router";
import { environment } from "src/environments/environment";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService, private _router: Router) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    if (req.url.startsWith(environment.apiRoot)) {
      const accessToken = this.authService.getAccessToken();
      const headers = req.headers.set("Authorization", `Bearer ${accessToken}`);
      const authReq = req.clone({ headers });
      return next.handle(authReq).pipe(
        tap(
          () => {},
          error => {
            const respError = error as HttpErrorResponse;
            if (
              respError &&
              (respError.status === 401 || respError.status === 403)
            ) {
              // ToDo: Implement unauthorized as for now only one user will operate on this site
              // so there is no point to Implement this.
              this._router.navigate(["/unauthorized"]);
            }
          }
        )
      );
    } else {
      return next.handle(req);
    }
  }
}
