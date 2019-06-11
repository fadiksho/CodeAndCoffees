import { Injectable } from "@angular/core";
import { User } from "./user";
import { Router } from "@angular/router";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { environment } from "src/environments/environment";
import { Observable } from "rxjs";
import { map, tap } from "rxjs/operators";
import * as jwt_decode from "jwt-decode";
import { ToastrService } from "ngx-toastr";

@Injectable({
  providedIn: "root"
})
export class AuthService {
  currentUser: User | null;

  constructor(
    private router: Router,
    private http: HttpClient,
    private toastr: ToastrService
  ) {
    this.currentUser = this.getUser();
  }

  isLoggedIn(): boolean {
    if (this.currentUser && !this.currentUser.isExpired()) {
      return true;
    }
    return false;
  }

  login(userName: string, password: string): Observable<User> {
    return this.http
      .post(environment.apiRoot + "auth/token", {
        UserName: userName,
        Password: password
      })
      .pipe(
        map((res: any) => {
          return this.convertTokenToUser(res.token);
        }),
        tap(user => {
          if (user && user.access_token) {
            this.currentUser = user;
            localStorage.setItem("blogManagerUser", JSON.stringify(user));
            this.toastr.success(
              "Welcome " + user.user_Name,
              "Successfully LogedIn"
            );
          }
        })
      );
  }

  logout(displayMessage = true) {
    localStorage.removeItem("blogManagerUser");
    this.currentUser = null;
    if (displayMessage) {
      this.toastr.success("GoodBye", "Successfully LogedOut");
    }
  }

  redirectToLoging() {
    this.router.navigate(["/login"]);
  }
  private convertTokenToUser(token: string): User {
    const jwtJson = jwt_decode(token);
    const user = {
      id_token: jwtJson.jti,
      access_token: token,
      user_Name: jwtJson.sub,
      start_at: new Date(jwtJson.nbf * 1000),
      expires_at: new Date(jwtJson.exp * 1000)
    };

    return Object.assign(new User(), user);
  }

  private getUser(): User | null {
    const userFromCache = JSON.parse(
      localStorage.getItem("blogManagerUser")
    ) as User;
    if (userFromCache) {
      const user = {
        id_token: userFromCache.id_token,
        access_token: userFromCache.access_token,
        user_Name: userFromCache.user_Name,
        start_at: new Date(userFromCache.start_at),
        expires_at: new Date(userFromCache.expires_at)
      };
      return Object.assign(new User(), user);
    }
    return null;
  }
}
