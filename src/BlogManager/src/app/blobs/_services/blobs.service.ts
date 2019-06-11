import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { environment } from "src/environments/environment";
import { SharedService } from "src/app/shared/shared.service";
import { Observable } from "rxjs";
import { PaggingQuery } from "src/app/shared/_model/paggingQuery";
import { PaggingResult } from "src/app/shared/_model/paggingResult";
import { Blob } from "../_model/blob";
@Injectable({
  providedIn: "root"
})
export class BlobsService {
  apiRoot = environment.apiRoot + "blob/";

  constructor(private http: HttpClient, private sharedService: SharedService) {}

  getBlob(blobId: number): Observable<Blob> {
    return this.http.get<Blob>(this.apiRoot + blobId);
  }

  getBlobPage(query: PaggingQuery): Observable<PaggingResult<Blob>> {
    return this.http.get<PaggingResult<Blob>>(
      this.apiRoot + "?" + this.sharedService.toQueryString(query)
    );
  }

  upload(file: File) {
    const formData = new FormData();
    formData.append("file", file);

    return this.http.post(this.apiRoot, formData);
  }

  deleteBlob(blobId: number): Observable<void> {
    return this.http.delete<void>(this.apiRoot + blobId);
  }
}
