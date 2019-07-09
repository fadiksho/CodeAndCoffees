import { NgModule } from "@angular/core";
import { PushNotificationAddComponent } from "./push-notification-add/push-notification-add.component";
import { Routes, RouterModule } from "@angular/router";
import { SharedModule } from "../shared/shared.module";
import { AutosizeModule } from "ngx-autosize";
import { PushNotificationService } from "./_services/push-notification.service";

const pushNotitficationRoutes: Routes = [
  { path: "", component: PushNotificationAddComponent }
];

@NgModule({
  imports: [
    SharedModule,
    RouterModule.forChild(pushNotitficationRoutes),
    AutosizeModule
  ],
  declarations: [PushNotificationAddComponent],
  providers: [PushNotificationService]
})
export class PushNotificationModule {}
