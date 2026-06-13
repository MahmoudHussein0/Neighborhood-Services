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

  // Enrichment returned only by GET /disputes/{id} (details view).
  customerUserId?: string | null;
  technicianUserId?: string | null;
  customerName?: string | null;
  technicianName?: string | null;

  escrowId?: number | null;
  escrowStatus?: string | null;   // Held | Released | Refunded
  escrowAmount?: number | null;
}