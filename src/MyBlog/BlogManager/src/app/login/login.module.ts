import { NgModule } from '@angular/core';
import { LoginComponent } from './login/login.component';
import { SharedModule } from '../shared/shared.module';
import { RouterModule, Routes } from '@angular/router';

const loginRoutes: Routes = [{ path: '', component: LoginComponent }];

@NgModule({
  declarations: [LoginComponent],
  imports: [SharedModule, RouterModule.forChild(loginRoutes)],
})
export class LoginModule {}
