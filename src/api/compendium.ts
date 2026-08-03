import { urlUtils } from "logitar-js";

import type { Language } from "@/types/languages";
import type { SearchResults } from "@/types/search";
import { get } from ".";

export async function getCompendiumLanguages(): Promise<SearchResults<Language>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/compendium/languages" }).buildRelative();
  return (await get<SearchResults<Language>>(url)).data;
}
