import { Component, OnInit } from "@angular/core";
import { AuthService } from "../auth.service";
@Component({
  selector: "app-auth-callback",
  templateUrl: "./auth-callback.component.html",
  styleUrls: ["./auth-callback.component.scss"]
})
export class AuthCallbackComponent implements OnInit {
  constructor(private authService: AuthService) {}

  ngOnInit() {
    if (this.authService.isLoggedIn()) {
      this.authService.navigateAfterSignIn();
    } else {
      this.authService.completeAuthentication();
    }
  }
}
