import { Injectable } from "@angular/core";

@Injectable({
  providedIn: "root"
})
export class SharedService {
  constructor() {}

  toQueryString(obj: any): string {
    const parts = [];
    for (const property in obj) {
      if (obj.hasOwnProperty(property)) {
        const value = obj[property];
        if (value != null && value !== undefined) {
          parts.push(
            encodeURIComponent(property) + "=" + encodeURIComponent(value)
          );
        }
      }
    }
    return parts.join("&");
  }
}
