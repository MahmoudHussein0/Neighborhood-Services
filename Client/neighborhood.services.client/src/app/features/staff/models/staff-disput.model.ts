export interface DisputeDto {
  id: number;
  bookingId: number;
  raisedByUserId: string;
  resolvedByStaffId?: number | null;

  disputeType: string;
  reason: string;
  resolution?: string | null;

  status: string;

  createdAt: string;
  resolvedAt?: string | null;
}