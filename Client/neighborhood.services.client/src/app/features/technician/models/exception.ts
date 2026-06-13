export interface Exception {
    id: number,
    technicianId: number;
    date: string;
    isAvailable: boolean;
    startTime: string;
    endTime: string;
    reason: string;
}