import { NgModule } from "@angular/core";
import { NgbTabsetModule, NgbCollapseModule } from "@ng-bootstrap/ng-bootstrap";
@NgModule({
  imports: [NgbTabsetModule],
  exports: [NgbTabsetModule, NgbCollapseModule],
  providers: []
})
export class BootstrapModule {}
