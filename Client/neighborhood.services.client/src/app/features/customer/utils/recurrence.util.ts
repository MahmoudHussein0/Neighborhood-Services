import { RecurringBooking } from '../models/recurring-booking.model';

const DAY_INDEX: Record<string, number> = {
  Sunday: 0, Monday: 1, Tuesday: 2, Wednesday: 3, Thursday: 4, Friday: 5, Saturday: 6,
};

/** Parse "yyyy-MM-dd" + "HH:mm:ss" into a local Date. */
function atTime(dateOnly: string, timeOfDay: string): Date {
  const [y, m, d] = dateOnly.split('-').map(Number);
  const [hh = 0, mm = 0] = (timeOfDay ?? '').split(':').map(Number);
  return new Date(y, (m ?? 1) - 1, d ?? 1, hh, mm, 0, 0);
}

function isOccurrence(candidate: Date, rb: RecurringBooking): boolean {
  switch (rb.pattern) {
    case 'Daily': return true;
    case 'Weekly': return rb.dayOfWeek != null && candidate.getDay() === DAY_INDEX[rb.dayOfWeek];
    case 'Monthly': return rb.dayOfMonth != null && candidate.getDate() === rb.dayOfMonth;
  }
}

/**
 * The next `count` occurrence datetimes strictly in the future, projected from the schedule.
 * Returns [] for Cancelled/Paused arrangements (no visits will be generated).
 */
export function nextOccurrences(rb: RecurringBooking, count: number, from: Date = new Date()): Date[] {
  if (rb.status === 'Cancelled' || rb.status === 'Paused') return [];

  const start = atTime(rb.startDate, rb.timeOfDay);
  const end = rb.endDate ? atTime(rb.endDate, '23:59:59') : null;

  // Walk forward day-by-day from the later of (start day, today).
  const begin = from > start ? from : start;
  const cursor = new Date(begin.getFullYear(), begin.getMonth(), begin.getDate());

  const out: Date[] = [];
  for (let i = 0; i < 366 * 2 && out.length < count; i++) {
    const day = new Date(cursor);
    day.setDate(cursor.getDate() + i);

    if (!isOccurrence(day, rb)) continue;

    const occ = new Date(day);
    occ.setHours(start.getHours(), start.getMinutes(), 0, 0);

    if (occ < start) continue;     // before the arrangement starts
    if (occ <= from) continue;     // not strictly in the future
    if (end && occ > end) break;   // past the end date
    out.push(occ);
  }
  return out;
}

/** The single next occurrence, or null. */
export function nextOccurrence(rb: RecurringBooking, from: Date = new Date()): Date | null {
  return nextOccurrences(rb, 1, from)[0] ?? null;
}
