// Read-only catalog types (Category / ProblemType are owned by Alaa's endpoints).

export interface Category {
  id: number;
  name: string;
  icon: string;
}

export interface ProblemType {
  id: number;
  name: string;
  description: string;
  minPrice: number;
  maxPrice: number;
}

// GET /api/categories/{id} — a category WITH its problem types
export interface CategoryDetails {
  id: number;
  name: string;
  icon: string;
  problemTypes: ProblemType[];
}
