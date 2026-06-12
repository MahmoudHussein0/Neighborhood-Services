/**
 * Builds a Google Maps URL that drops a pin at the given coordinates.
 * No API key needed — opens the native Maps app on mobile, maps.google.com on desktop.
 * Returns null when coordinates are missing/zero so callers can hide the link.
 */
export function googleMapsUrl(latitude?: number | null, longitude?: number | null): string | null {
  if (latitude == null || longitude == null) return null;
  if (latitude === 0 && longitude === 0) return null;
  return `https://www.google.com/maps/search/?api=1&query=${latitude},${longitude}`;
}
