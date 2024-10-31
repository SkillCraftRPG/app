export function getWorldSlug(): string | undefined {
  const parts: string[] = window.location.href.split("/");
  const index: number = parts.indexOf("worlds");
  return index < 0 ? undefined : parts[index + 1] || undefined;
} // TODO(fpion): unit tests
