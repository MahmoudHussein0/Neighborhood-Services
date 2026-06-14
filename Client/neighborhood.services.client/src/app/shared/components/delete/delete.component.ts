import { Component, ElementRef, input, InputSignal, viewChild } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-delete',
  imports: [TranslatePipe],
  templateUrl: './delete.component.html',
  styleUrl: './delete.component.css',
})
export class DeleteComponent {



  modalId = input.required<string>();
  closeBtn = viewChild<ElementRef>('closeBtn');
  close() {
    this.closeBtn()?.nativeElement.click();
  }







}
