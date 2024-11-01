import { urlUtils } from "logitar-js";

import type { CasteModel, CreateOrReplaceCastePayload, SearchCastesPayload } from "@/types/castes";
import type { SearchResults } from "@/types/search";
import { get, post } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/castes/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/castes" });
}

export async function createCaste(payload: CreateOrReplaceCastePayload): Promise<CasteModel> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceCastePayload, CasteModel>(url, payload)).data;
}

export async function searchCastes(payload: SearchCastesPayload): Promise<SearchResults<CasteModel>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery(
      "search_terms",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery("skill", payload.skill ?? "")
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<CasteModel>>(url)).data;
}
