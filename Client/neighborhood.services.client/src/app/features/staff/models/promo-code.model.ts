export interface PromoCode {
  id: number;
  code: string;
  discountPercentage: number;
  maxUses: number;
  usedCount: number;
  expiresAt: string;
  isActive: boolean;
  createdAt: string;
}
