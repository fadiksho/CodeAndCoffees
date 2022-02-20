import { NgModule } from "@angular/core";
import { NgbNavModule, NgbCollapseModule } from "@ng-bootstrap/ng-bootstrap";
@NgModule({
  imports: [NgbNavModule],
  exports: [NgbNavModule, NgbCollapseModule],
  providers: []
})
export class BootstrapModule {}
