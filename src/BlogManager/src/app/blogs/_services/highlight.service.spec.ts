import { TestBed } from "@angular/core/testing";

import { HtmlService } from "./html.service";

describe("HighlightService", () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it("should be created", () => {
    const service: HtmlService = TestBed.get(HtmlService);
    expect(service).toBeTruthy();
  });
});
