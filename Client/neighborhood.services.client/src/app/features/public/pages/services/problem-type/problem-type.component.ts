import { Component, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProblemTypeService } from '../../../../../core/services/problem-type.service';
import { ProblemType } from '../../../models/problem-type';
import { HowitworksComponent } from "../../home/howitworks/howitworks.component";
import { TranslatePipe } from '@ngx-translate/core';
import { LangService } from '../../../../../core/services/lang.service';
import { skip } from 'rxjs';

@Component({
  selector: 'app-problem-type',
  imports: [HowitworksComponent, TranslatePipe],
  templateUrl: './problem-type.component.html',
  styleUrl: './problem-type.component.css',
})
export class ProblemTypeComponent implements OnInit {

  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly problemTypeService = inject(ProblemTypeService);
  private readonly langService = inject(LangService);

  problemDetails: WritableSignal<ProblemType> = signal<ProblemType>({} as ProblemType);

  problemId!: number;




  ngOnInit(): void {
    this.getProblemId();
    this.langService.lang$
      .subscribe(() => {
        this.getProblemType();
      });
  }


  getProblemId(): void {
    this.problemId = Number(this.activatedRoute.snapshot.paramMap.get("id"));
  }


  getProblemType(): void {
    this.problemTypeService.getProblemTypeById(this.problemId).subscribe({
      next: (res => {
        console.log(res);
        this.problemDetails.set(res);
      }),
      error: (err => {
        console.log(err);

      })
    })
  }



}
