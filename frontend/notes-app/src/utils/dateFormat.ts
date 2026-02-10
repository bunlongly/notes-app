/**
 * Date formatting utilities for displaying timestamps
 */

import {
  formatDistanceToNow,
  format,
  isToday,
  isYesterday,
  isThisYear
} from 'date-fns';

export function formatTimestamp(dateString: string): string {
  const date = new Date(dateString);

  // If today, show time
  if (isToday(date)) {
    return `Today at ${format(date, 'h:mm a')}`;
  }

  // If yesterday
  if (isYesterday(date)) {
    return `Yesterday at ${format(date, 'h:mm a')}`;
  }

  // If this year, show date without year
  if (isThisYear(date)) {
    return format(date, 'MMM d, h:mm a');
  }

  // Otherwise show full date
  return format(date, 'MMM d, yyyy, h:mm a');
}

export function formatRelativeTime(dateString: string): string {
  const date = new Date(dateString);
  return formatDistanceToNow(date, { addSuffix: true });
}

export function formatFullTimestamp(dateString: string): string {
  const date = new Date(dateString);
  return format(date, 'PPpp'); // Example: "Apr 29, 2023, 3:15:30 PM"
}
