import { urlUtils } from "logitar-js";

import type { CreateOrReplaceLanguagePayload, LanguageModel, SearchLanguagesPayload } from "@/types/languages";
import type { SearchResults } from "@/types/search";
import { get, post } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/languages/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/languages" });
}

export async function createLanguage(payload: CreateOrReplaceLanguagePayload): Promise<LanguageModel> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceLanguagePayload, LanguageModel>(url, payload)).data;
}

export async function searchLanguages(payload: SearchLanguagesPayload): Promise<SearchResults<LanguageModel>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery(
      "search_terms",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery("script", payload.script ?? "")
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<LanguageModel>>(url)).data;
}
