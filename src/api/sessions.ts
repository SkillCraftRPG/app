import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { Session } from "@/types/account";
import { _delete, get } from "./index";

export async function listActiveSessions(): Promise<SearchResults<Session>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/sessions" }).buildRelative();
  return (await get<SearchResults<Session>>(url)).data;
}

export async function signOutById(id: string): Promise<void> {
  const url: string = new urlUtils.UrlBuilder({ path: "/sessions/{id}" }).setParameter("id", id).buildRelative();
  await _delete(url);
}
