import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { ShellComponent } from "./home/shell/shell.component";
import { WelcomeComponent } from "./home/welcome/welcome.component";
import { PageNotFoundComponent } from "./home/page-not-found.component";
import { AuthGuard } from "./_guards/auth.guard";

const routes: Routes = [
  {
    path: "",
    component: ShellComponent,
    children: [
      { path: "welcome", component: WelcomeComponent },
      {
        path: "blog",
        canActivateChild: [AuthGuard],
        loadChildren: () => import('./blogs/blog.module').then(m => m.BlogModule)
      },
      {
        path: "blob",
        canActivateChild: [AuthGuard],
        loadChildren: () => import('./blobs/blob.module').then(m => m.BlobModule)
      },
      {
        path: "pushnotification",
        canActivateChild: [AuthGuard],
        loadChildren:
          () => import('./push-notification/push-notification.module').then(m => m.PushNotificationModule)
      },
      {
        path: "login",
        loadChildren: () => import('./user/user.module').then(m => m.UserModule)
      },
      { path: "", redirectTo: "welcome", pathMatch: "full" }
    ]
  },
  { path: "**", component: PageNotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
