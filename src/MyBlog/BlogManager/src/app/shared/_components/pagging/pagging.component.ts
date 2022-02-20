import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-pagging',
  templateUrl: './pagging.component.html',
  styleUrls: ['./pagging.component.scss'],
})
export class PaggingComponent implements OnInit {
  pages: number[] = [];

  @Input() totalItems!: number;
  @Input() pageSize!: number;
  @Input() currentPage!: number;
  @Input() totalPages!: number;
  @Input() hasNext!: boolean;
  @Input() hasPrevious!: boolean;
  @Output() pageChange = new EventEmitter<number>();

  constructor() {}

  ngOnInit() {
    for (let i = 1; i <= this.totalPages; i++) {
      this.pages.push(i);
    }
  }

  pageChanged(page: number) {
    this.pageChange.emit(page);
  }
}
