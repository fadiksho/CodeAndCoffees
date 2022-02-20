import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { WelcomeComponent } from './home/welcome/welcome.component';
import { ShellComponent } from './home/shell/shell.component';
import { NavComponent } from './home/nav/nav.component';
import { PageNotFoundComponent } from './home/page-not-found/page-not-found.component';
import { SharedModule } from './shared/shared.module';
import { ToastrModule } from 'ngx-toastr';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  declarations: [
    AppComponent,
    WelcomeComponent,
    ShellComponent,
    NavComponent,
    PageNotFoundComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    SharedModule,
    ToastrModule.forRoot(),
    HttpClientModule,
  ],
  // providers: [{ provide: APP_BASE_HREF, useValue: '/admin' }],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
