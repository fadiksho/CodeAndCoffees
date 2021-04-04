import { Component, OnInit, ElementRef, ViewChild } from "@angular/core";
import { PaggingQuery } from "src/app/shared/_model/paggingQuery";
import { PaggingResult } from "src/app/shared/_model/paggingResult";
import { BlobsService } from "../_services/blobs.service";
import { ToastrService } from "ngx-toastr";
import { Blob } from "../_model/blob";
@Component({
  selector: "app-blob-list",
  templateUrl: "./blob-list.component.html",
  styleUrls: ["./blob-list.component.scss"]
})
export class BlobListComponent implements OnInit {
  @ViewChild("fileInput") fileInput: ElementRef;

  query: PaggingQuery = {
    page: 1,
    pageSize: 10
  };

  paggingResult: PaggingResult<Blob>;
  constructor(
    private blobService: BlobsService,
    private toastr: ToastrService // private progressService: ProgressService
  ) {}

  ngOnInit() {
    this.getBlobPage();
  }

  onPageChange(page: number) {
    this.query.page = page;
    this.getBlobPage();
  }

  private getBlobPage(): void {
    this.blobService.getBlobPage(this.query).subscribe(
      result => {
        this.paggingResult = result;
      },
      error => {
        this.toastr.error(error.error, "Error");
      }
    );
  }

  deleteBlob(blobId: number): void {
    if (blobId) {
      if (confirm("Confirm The Delete!")) {
        this.blobService.deleteBlob(blobId).subscribe(
          () => {
            this.paggingResult.tResult = this.paggingResult.tResult.filter(
              value => value.id !== blobId
            );
          },
          (error: any) => {
            this.toastr.error(
              "Blob Delete Failed! error message:" + error.error,
              "Error"
            );
          }
        );
      }
    }
  }

  uploadFile() {
    const nativeElement: HTMLInputElement = this.fileInput.nativeElement;

    this.blobService.upload(nativeElement.files[0]).subscribe(value => {
      this.toastr.success("Upload File Filnished", "Success");
    });
  }

  copyFileLink(link: string) {
    // Create new element
    const el = document.createElement("textarea");
    // Set value (string to be copied)
    el.value = link;
    // Set non-editable to avoid focus and move outside of view
    el.setAttribute("readonly", "");
    document.body.appendChild(el);
    // Select text inside element
    el.select();
    // Copy text to clipboard
    document.execCommand("copy");
    // Remove temporary element
    document.body.removeChild(el);
    this.toastr.success(link, "Successfully Copied");
  }
}
