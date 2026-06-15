export interface CategoryDetails {
    id: number;
    name: string;
    icon: string;
    problemTypes: ProblemTypes[];
}

export interface ProblemTypes {
    id: number;
    name: string;
    nameAr: string,
    nameEn: string,
    description: string;
    descriptionEn: string;
    descriptionAr: string;
    minPrice: number;
    maxPrice: number;
    imageUrl: string;
}