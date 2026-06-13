export interface CategoryDetails {
    id: number;
    name: string;
    icon: string;
    problemTypes: ProblemTypes[];
}

export interface ProblemTypes {
    id: number;
    name: string;
    description: string;
    minPrice: number;
    maxPrice: number;
    imageUrl: string;
}