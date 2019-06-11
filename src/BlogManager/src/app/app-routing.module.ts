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
        loadChildren: "./blogs/blog.module#BlogModule"
      },
      {
        path: "blob",
        canActivateChild: [AuthGuard],
        loadChildren: "./blobs/blob.module#BlobModule"
      },
      {
        path: "pushnotification",
        canActivateChild: [AuthGuard],
        loadChildren:
          "./push-notification/push-notification.module#PushNotificationModule"
      },
      {
        path: "login",
        loadChildren: "./user/user.module#UserModule"
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
