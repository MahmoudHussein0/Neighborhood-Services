import { Component } from '@angular/core';
import { AvailiabilityComponent } from "../availiability/availiability.component";
import { ExceptionComponent } from "../exception/exception.component";

@Component({
  selector: 'app-availiability-and-exception',
  imports: [AvailiabilityComponent, ExceptionComponent],
  templateUrl: './availiability-and-exception.component.html',
  styleUrl: './availiability-and-exception.component.css',
})
export class AvailiabilityAndExceptionComponent { }
