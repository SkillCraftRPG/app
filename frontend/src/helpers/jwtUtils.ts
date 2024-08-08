export function decode(token: string): unknown {
  const base64Url: string = token.split(".")[1];
  const base64: string = base64Url.replace(/-/g, "+").replace(/_/g, "/");
  const json = decodeURIComponent(
    atob(base64)
      .split("")
      .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
      .join(""),
  );
  return JSON.parse(json);
} // ISSUE: https://github.com/SkillCraftRPG/app/issues/23
