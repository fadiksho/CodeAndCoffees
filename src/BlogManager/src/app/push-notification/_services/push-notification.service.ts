import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { Observable } from "rxjs";
import { PushNotificationPayload } from "../_model/push-notification-payload";
import { HttpClient } from "@angular/common/http";

@Injectable()
export class PushNotificationService {
  apiRoot = environment.apiRoot + "subscribers/";
  constructor(private http: HttpClient) {}

  newPushNotification(payload: PushNotificationPayload): Observable<any> {
    return this.http.post(this.apiRoot + "NewPushNotification", payload);
  }
  getPushNotificationSubscribersCount(): Observable<number> {
    return this.http.get<number>(this.apiRoot + "getSubscribersCount");
  }
}
