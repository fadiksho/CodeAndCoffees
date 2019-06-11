import { Component, OnInit } from "@angular/core";
import { AuthService } from "src/app/user/auth.service";

@Component({
  selector: "app-nav",
  templateUrl: "./nav.component.html",
  styleUrls: ["./nav.component.scss"]
})
export class NavComponent implements OnInit {
  isCollapsed = true;

  constructor(public auth: AuthService) {}

  ngOnInit() {}
}
