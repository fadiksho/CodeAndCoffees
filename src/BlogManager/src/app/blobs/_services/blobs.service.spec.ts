import { TestBed } from "@angular/core/testing";

import { BlobsService } from "./blobs.service";

describe("BlobsService", () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it("should be created", () => {
    const service: BlobsService = TestBed.get(BlobsService);
    expect(service).toBeTruthy();
  });
});
