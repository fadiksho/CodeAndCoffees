import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BootstrapModule } from './bootstrap.module';
import { PaggingComponent } from './_components/pagging/pagging.component';

@NgModule({
  imports: [CommonModule],
  declarations: [PaggingComponent],
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PaggingComponent,
    BootstrapModule,
  ],
  providers: [DatePipe],
})
export class SharedModule {}
