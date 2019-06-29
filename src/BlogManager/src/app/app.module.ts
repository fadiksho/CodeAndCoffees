import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { ShellComponent } from "./home/shell/shell.component";
import { WelcomeComponent } from "./home/welcome/welcome.component";
import { NavComponent } from "./home/nav/nav.component";
import { PageNotFoundComponent } from "./home/page-not-found.component";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { SharedModule } from "./shared/shared.module";
import { ToastrModule } from "ngx-toastr";
import { AuthInterceptor } from "./_interceptors/auth.interceptor";
import { AuthCallbackComponent } from "./user/auth-callback/auth-callback.component";

@NgModule({
  declarations: [
    AppComponent,
    ShellComponent,
    WelcomeComponent,
    NavComponent,
    PageNotFoundComponent,
    AuthCallbackComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    SharedModule,
    ToastrModule.forRoot(),
    HttpClientModule,
    AppRoutingModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
