// Mirrors TechnicianAvailabilityDetailsDTO (GET /api/technitianavailability/{technicianId}).
export interface TechnicianAvailabilitySlot {
  dayOfWeek: string;   // "Monday" (System.DayOfWeek serialized as string)
  startTime: string;   // "HH:mm:ss"
  endTime: string;     // "HH:mm:ss"
}
