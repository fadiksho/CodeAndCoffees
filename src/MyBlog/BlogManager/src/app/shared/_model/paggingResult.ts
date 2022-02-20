export class PaggingResult<T> {
  tResult!: T[];
  totalItems!: number;
  pageSize!: number;
  currentPage!: number;
  totalPages!: number;
  hasNext!: boolean;
  hasPrevious!: boolean;
}
