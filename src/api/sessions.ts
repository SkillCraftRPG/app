import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { Session } from "@/types/account";
import { get } from "./index";

export async function listSessions(): Promise<SearchResults<Session>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/sessions" }).buildRelative();
  return (await get<SearchResults<Session>>(url)).data;
}
