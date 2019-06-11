import { NgModule } from "@angular/core";
import { LoginComponent } from "./login/login.component";
import { SharedModule } from "../shared/shared.module";
import { RouterModule, Routes } from "@angular/router";

const userRoutes: Routes = [{ path: "", component: LoginComponent }];

@NgModule({
  imports: [SharedModule, RouterModule.forChild(userRoutes)],
  declarations: [LoginComponent]
})
export class UserModule {}
