export function formatRelativeTime(value: string | Date, locale: string, justNow: string, now = new Date()): string {
  const date = value instanceof Date ? value : new Date(value);
  const differenceInSeconds = Math.round((date.getTime() - now.getTime()) / 1_000);
  const absoluteDifference = Math.abs(differenceInSeconds);

  if (absoluteDifference < 45) {
    return justNow;
  }

  const formatter = new Intl.RelativeTimeFormat(locale, { numeric: "always" });

  if (absoluteDifference < 60 * 60) {
    return formatter.format(Math.round(differenceInSeconds / 60), "minute");
  }

  if (absoluteDifference < 60 * 60 * 24) {
    return formatter.format(Math.round(differenceInSeconds / (60 * 60)), "hour");
  }

  return formatter.format(Math.round(differenceInSeconds / (60 * 60 * 24)), "day");
}
