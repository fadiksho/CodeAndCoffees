import { PaggingQuery } from "../../shared/_model/paggingQuery";

export interface BlogQuery extends PaggingQuery {
  tags: string[];
  onlyPublished: boolean;
}
