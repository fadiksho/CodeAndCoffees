import { Component, OnInit } from "@angular/core";
import { AuthService } from "./user/auth.service";
import { Router } from "@angular/router";

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"]
})
export class AppComponent implements OnInit {
  title = "Blog Manager";

  constructor(private authService: AuthService, private router: Router) {
    if (window.location.href.indexOf("?postLogout=true") > 0) {
      this.authService.signoutRedirectCallback().then(() => {
        let url = this.router.url.substring(0, this.router.url.indexOf("?"));
        this.router.navigateByUrl(url);
      });
    }
  }

  ngOnInit() {}
}
