export interface Pricing {
    id: number;
    nationalId: string;
    experience: string;
    rating: number;
    maxTravelDistance: number;
    verificationStatus: string;
    problemTypeName: string;
    problemTypeDescription: string;
    problemTypeId: number;
    techMinPrice: number;
    techMaxPrice: number;
}