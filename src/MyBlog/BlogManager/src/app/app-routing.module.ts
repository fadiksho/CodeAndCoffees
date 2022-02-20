import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PageNotFoundComponent } from './home/page-not-found/page-not-found.component';
import { ShellComponent } from './home/shell/shell.component';
import { WelcomeComponent } from './home/welcome/welcome.component';
import { UserGuard } from './_auth/user.guard';

const routes: Routes = [
  {
    path: '',
    component: ShellComponent,
    children: [
      { path: '', component: WelcomeComponent, pathMatch: 'full' },
      { path: 'welcome', component: WelcomeComponent },
      {
        path: 'pushnotification',
        canActivateChild: [UserGuard],
        loadChildren: () =>
          import('./push-notification/push-notification.module').then(
            (m) => m.PushNotificationModule
          ),
      },
      {
        path: 'login',
        canActivateChild: [UserGuard],
        loadChildren: () =>
          import('./login/login.module').then((m) => m.LoginModule),
      },
      { path: '**', component: PageNotFoundComponent },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
