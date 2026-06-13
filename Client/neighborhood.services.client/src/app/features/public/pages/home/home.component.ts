import { Component } from '@angular/core';
import { HeroSectionComponent } from "./hero-section/hero-section.component";
import { HowitworksComponent } from "./howitworks/howitworks.component";
import { OurservicesComponent } from "./ourservices/ourservices.component";

@Component({
  selector: 'app-home',
  imports: [HeroSectionComponent, HowitworksComponent, OurservicesComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent { }
