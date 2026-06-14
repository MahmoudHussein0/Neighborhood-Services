export interface ReviewAnalysisDto {
  id: number;
  reviewId: number;
  sentiment: 'Positive' | 'Neutral' | 'Negative';
  isFlagged: boolean;
  qualityScore: number;
  createdAt: string;
}