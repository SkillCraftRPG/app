import { urlUtils } from "logitar-js";

import type { Customization } from "@/types/customizations";
import type { Language } from "@/types/languages";
import type { SearchResults } from "@/types/search";
import { get } from ".";

export async function getCompendiumCustomizations(): Promise<SearchResults<Customization>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/compendium/customizations" }).buildRelative();
  return (await get<SearchResults<Customization>>(url)).data;
}

export async function getCompendiumLanguages(): Promise<SearchResults<Language>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/compendium/languages" }).buildRelative();
  return (await get<SearchResults<Language>>(url)).data;
}
