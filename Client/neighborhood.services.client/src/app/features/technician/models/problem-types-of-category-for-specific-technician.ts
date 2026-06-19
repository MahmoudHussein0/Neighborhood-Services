import { ProblemTypes } from "../../staff/models/category-details";

export interface ProblemTypesOfCategoryForSpecificTechnician {

    technicianCategoryId: number,
    id: number;
    name: string;
    nameAr: string;
    nameEn: string;
    image: null;
    icon: string;
    technicians: number;
    problemTypes: ProblemTypes[];
}

