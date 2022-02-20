import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from 'src/app/_auth/user.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss'],
})
export class NavComponent implements OnInit {
  isCollapsed = true;

  constructor(public userService: UserService, router: Router) {
    router.events.subscribe(() => {
      this.isCollapsed = true;
    });
  }

  ngOnInit(): void {}
}
