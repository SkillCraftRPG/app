import { urlUtils } from "logitar-js";

import type { Caste } from "@/types/castes";
import type { Customization } from "@/types/customizations";
import type { Education } from "@/types/educations";
import type { Language } from "@/types/languages";
import type { Script } from "@/types/scripts";
import type { SearchResults } from "@/types/search";
import type { Talent } from "@/types/talents";
import { get } from ".";

export async function getCompendiumCastes(): Promise<SearchResults<Caste>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/compendium/castes" }).buildRelative();
  return (await get<SearchResults<Caste>>(url)).data;
}

export async function getCompendiumCustomizations(): Promise<SearchResults<Customization>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/compendium/customizations" }).buildRelative();
  return (await get<SearchResults<Customization>>(url)).data;
}

export async function getCompendiumEducations(): Promise<SearchResults<Education>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/compendium/educations" }).buildRelative();
  return (await get<SearchResults<Education>>(url)).data;
}

export async function getCompendiumLanguages(): Promise<SearchResults<Language>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/compendium/languages" }).buildRelative();
  return (await get<SearchResults<Language>>(url)).data;
}

export async function getCompendiumScripts(): Promise<SearchResults<Script>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/compendium/scripts" }).buildRelative();
  return (await get<SearchResults<Script>>(url)).data;
}

export async function getCompendiumTalents(): Promise<SearchResults<Talent>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/compendium/talents" }).buildRelative();
  return (await get<SearchResults<Talent>>(url)).data;
}
