import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { environment } from "src/environments/environment";
import {
  UserManager,
  UserManagerSettings,
  WebStorageStateStore,
  User,
  Log
} from "oidc-client";

@Injectable({
  providedIn: "root"
})
export class AuthService {
  private user: User;
  private userManager: UserManager;
  private userManagerSettings = {
    ...environment.userManagerSettings,
    userStore: new WebStorageStateStore({ store: window.localStorage })
  } as UserManagerSettings;

  constructor(private router: Router) {
    Log.logger = console;
    this.userManager = new UserManager(this.userManagerSettings);
    this.userManager.getUser().then(user => {
      if (user && !user.expired) {
        this.user = user;
      }
    });
    this.userManager.events.addUserLoaded(args => {
      this.userManager.getUser().then(user => {
        if (user && !user.expired) {
          this.user = user;
        }
      });
    });
  }

  isLoggedIn(): boolean {
    return this.user && this.user.access_token && !this.user.expired;
  }

  login(url: string): Promise<any> {
    return this.userManager.signinRedirect({ state: { returnUrl: url } });
  }

  logout() {
    this.userManager.signoutRedirect();
  }

  getAccessToken(): string {
    return this.user ? this.user.access_token : "";
  }

  // ToDo: Split This code
  completeAuthentication(): Promise<void> {
    return this.userManager.signinRedirectCallback().then(user => {
      if (user.state && user.state.returnUrl) {
        this.navigateAfterSignIn(user.state.returnUrl);
      } else {
        this.navigateAfterSignIn();
      }
    });
  }

  signoutRedirectCallback(): Promise<any> {
    return this.userManager.signoutRedirectCallback();
  }

  navigateAfterSignIn(returnUrl = "/") {
    this.router.navigate([returnUrl]);
  }
}
