export function getWorldSlug(href?: string): string | undefined {
  const parts: string[] = (href ?? window.location.href).split("/");
  const index: number = parts.indexOf("worlds");
  return index < 0 ? undefined : parts[index + 1] || undefined;
}
