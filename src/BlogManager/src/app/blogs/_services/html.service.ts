import { Injectable } from "@angular/core";
import * as Highlight from "highlight.js";

@Injectable()
export class HtmlService {
  constructor() {}

  private highlight(codeBlock: any): void {
    Highlight.configure({
      tabReplace: " ",
      useBR: false
    });
    Highlight.highlightBlock(codeBlock);
  }

  getHtmlValueForEdit(html: string): string {
    const parser = new DOMParser();
    const doc = parser.parseFromString(html, "text/html");
    const codeElements = doc.getElementsByTagName("code");
    for (let i = 0; i < codeElements.length; i++) {
      codeElements[i].innerHTML = codeElements[i].innerText;
    }
    return doc.body.innerHTML;
  }

  getHtmlValueForPreview(html: string): string {
    const parser = new DOMParser();
    const doc = parser.parseFromString(html, "text/html");
    const codeElements = doc.getElementsByTagName("code");
    for (let i = 0; i < codeElements.length; i++) {
      codeElements[i].innerHTML = codeElements[i].innerHTML
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");
      this.highlight(codeElements[i]);
    }
    return doc.body.innerHTML;
  }
}
