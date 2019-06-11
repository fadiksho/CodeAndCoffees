import { Component, OnInit, Input, EventEmitter, Output } from "@angular/core";
import { BlogQuery } from "../_model/blogQuery";

@Component({
  selector: "app-filter",
  templateUrl: "./filter.component.html",
  styleUrls: ["./filter.component.scss"]
})
export class FilterComponent implements OnInit {
  @Input() onlyPublished: boolean;
  @Output() getOnlyPublished = new EventEmitter<boolean>();

  constructor() {}

  ngOnInit() {}

  togglePublished(value: boolean) {
    this.getOnlyPublished.emit(value);
  }
}
