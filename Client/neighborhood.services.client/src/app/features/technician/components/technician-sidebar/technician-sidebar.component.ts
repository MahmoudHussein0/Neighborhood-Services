import { Component, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { ActivatedRoute, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-technician-sidebar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './technician-sidebar.component.html',
  styleUrl: './technician-sidebar.component.css',
})
export class TechnicianSidebarComponent implements OnInit {


  private readonly activatedRoute = inject(ActivatedRoute);
  technicianId: WritableSignal<number> = signal<number>(0)
  collapsed = signal(false);

  ngOnInit(): void {
    this.getTechnicianId();
  }


  toggle() {
    this.collapsed.update(v => !v);
  }




  getTechnicianId() {
    this.activatedRoute.paramMap.subscribe({
      next: (urlParams => {
        this.technicianId.set(Number(urlParams.get("id")));

      })
    })
  }














}
