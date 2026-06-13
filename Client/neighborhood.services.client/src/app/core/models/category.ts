export interface Category {
    id: number;
    name: string;
    icon: string;
    image: string;
    technicians: number;
    problemTypes: ProblemType[];
}

interface ProblemType {
    id: number;
    name: string;
    description: string;
    minPrice: number;
    maxPrice: number;
}