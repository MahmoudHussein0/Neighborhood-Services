export interface Category {
    id: number;
    nameEn: string;
    nameAr: string;
    name: string,
    icon: string;
    image: string;
    technicians: number;
    problemTypes: ProblemType[];
}

interface ProblemType {
    id: number;
    name: string;
    nameAr: string;
    nameEn: string;
    description: string;
    minPrice: number;
    maxPrice: number;
}