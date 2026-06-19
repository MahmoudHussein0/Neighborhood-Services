import { Component } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-about',
  imports: [TranslatePipe],
    styles:`
.container{
margin-top:0px;
background:#F0F8FF !important;
}
.text-custom{
color: #0062ff !important;
}`,
  template: `
     <div class="container-fluid my-3 py-1" style="background:#F0F8FF !important;">
    <div class="row align-items-center">
        <div class="col-lg-6 col-md-6 order-2 order-md-1 mt-4 pt-2 mt-sm-0 opt-sm-0">
            <div class="row align-items-center">
                <div class="col-lg-6 col-md-6 col-6">
                    <div class="row">
                        <div class="col-lg-12 col-md-12 mt-4 pt-2">
                            <div class="card work-desk rounded border-0 shadow-lg overflow-hidden">
                                <img src="https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSdcMdr2YqYaDILKcVGSSAosPq9HS5Jz7DP5OXDpwstdl6E5gx4T9eXUmU&s=10" class="img-fluid" alt="Image" />
                                <div class="img-overlay bg-dark"></div>
                            </div>
                        </div>
                        <!--end col-->

                        
                    </div>
                    <!--end row-->
                </div>
                <!--end col-->

                <div class="col-lg-6 col-md-6 col-6">
                    <div class="row">
                        <div class="col-lg-12 col-md-12">
                            <div class="card work-desk rounded border-0 shadow-lg overflow-hidden">
                                <img src="https://api.cezma.cloud/storage/thumbnails/products/web/17765190281000604488.jpg" class="img-fluid" alt="Image" />
                                <div class="img-overlay bg-dark"></div>
                            </div>
                        </div>
                        <!--end col-->

                        <div class="col-lg-12 col-md-12 mt-4 pt-2">
                            <div class="card work-desk rounded border-0 shadow-lg overflow-hidden">
                                <img src="https://i0.wp.com/elmostafaa.com/wp-content/uploads/2025/08/%D9%86%D9%82%D8%A7%D8%B4%D8%A9-3.jpg?resize=1024%2C1024&ssl=1" class="img-fluid" alt="Image" />
                                <div class="img-overlay bg-dark"></div>
                            </div>
                        </div>
                        <!--end col-->
                    </div>
                    <!--end row-->
                </div>
                <!--end col-->
            </div>
            <!--end row-->
        </div>
        <!--end col-->

        <div class="col-lg-6 col-md-6 col-12 order-1 order-md-2">
            <div class="section-title ml-lg-5">
                <h5 class="text-custom font-weight-normal mb-3">{{'aboutUs.aboutUs' | translate}}</h5>
                <h4 class="title mb-4">
                   
                    {{'aboutUs.title' | translate}}
                </h4>
                <p class="text-muted mb-0"> {{'aboutUs.description' | translate}}</p>

                <div class="row">
                    <div class="col-lg-6 mt-4 pt-2">
                        <div class="media align-items-center rounded shadow p-3">
                            <i class="bi bi-check-circle h4 mb-0 text-custom"></i>
                            <h6 class="ml-3 mb-0 text-dark">{{'aboutUs.f1' | translate}}</h6>
                        </div>
                    </div>
                    <div class="col-lg-6 mt-4 pt-2">
                        <div class="media align-items-center rounded shadow p-3">
                            <i class="bi bi-bar-chart-steps h4 mb-0 text-custom"></i>
                            <h6 class="ml-3 mb-0">{{'aboutUs.f2' | translate}}</h6>
                        </div>
                    </div>
                    <div class="col-lg-6 mt-4 pt-2">
                        <div class="media align-items-center rounded shadow p-3">
                            <i class="fa fa-user h4 mb-0 text-custom"></i>
                            <h6 class="ml-3 mb-0">{{'aboutUs.f3' | translate}}</h6>
                        </div>
                    </div>
                    <div class="col-lg-6 mt-4 pt-2">
                        <div class="media align-items-center rounded shadow p-3">
                            <i class="bi bi-award-fill h4 mb-0 text-custom"></i>
                            <h6 class="ml-3 mb-0">{{'aboutUs.f4' | translate}}</h6>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!--end col-->
    </div>
    <!--enr row-->
</div>
  `,
})
export class AboutComponent {}
