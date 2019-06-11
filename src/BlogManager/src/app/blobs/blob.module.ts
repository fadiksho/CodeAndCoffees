import { NgModule } from "@angular/core";
import { BlobListComponent } from "./blob-list/blob-list.component";
import { RouterModule, Routes } from "@angular/router";
import { SharedModule } from "../shared/shared.module";
import { AutosizeModule } from "ngx-autosize";
import { BlobsService } from "./_services/blobs.service";
import { FileSizePipe } from "./_pipes/file-size.pipe";
import { BrowserXhrWithProgress } from "./_services/progress.service";
import { BrowserXhr } from "@angular/http";

const blobRoutes: Routes = [{ path: "", component: BlobListComponent }];

@NgModule({
  imports: [SharedModule, RouterModule.forChild(blobRoutes), AutosizeModule],
  declarations: [BlobListComponent, FileSizePipe],
  providers: [BlobsService]
})
export class BlobModule {}
