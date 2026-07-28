import { urlUtils } from "logitar-js";

import type { CreateOrReplaceLanguagePayload, SearchLanguagesPayload, UpdateLanguagePayload, Language } from "@/types/languages";
import type { SearchResults } from "@/types/search";
import { get, patch, post, put } from ".";

export async function createLanguage(payload: CreateOrReplaceLanguagePayload): Promise<Language> {
  const url: string = new urlUtils.UrlBuilder({ path: "/languages" }).buildRelative();
  return (await post<CreateOrReplaceLanguagePayload, Language>(url, payload)).data;
}

export async function readLanguage(id: string): Promise<Language> {
  const url: string = new urlUtils.UrlBuilder({ path: "/languages/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Language>(url)).data;
}

export async function replaceLanguage(id: string, payload: CreateOrReplaceLanguagePayload): Promise<Language> {
  const url: string = new urlUtils.UrlBuilder({ path: "/languages/{id}" }).setParameter("id", id).buildRelative();
  return (await put<CreateOrReplaceLanguagePayload, Language>(url, payload)).data;
}

export async function searchLanguages(payload: SearchLanguagesPayload): Promise<SearchResults<Language>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/languages" })
    .setQuery("ids", payload.ids)
    .setQuery("script", payload.scriptId ?? "")
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<Language>>(url)).data;
}

export async function updateLanguage(id: string, payload: UpdateLanguagePayload): Promise<Language> {
  const url: string = new urlUtils.UrlBuilder({ path: "/languages/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateLanguagePayload, Language>(url, payload)).data;
}
