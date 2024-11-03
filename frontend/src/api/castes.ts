import { urlUtils } from "logitar-js";

import type { CasteModel, CreateOrReplaceCastePayload, SearchCastesPayload } from "@/types/castes";
import type { SearchResults } from "@/types/search";
import { get, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/castes/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/castes" });
}

export async function createCaste(payload: CreateOrReplaceCastePayload): Promise<CasteModel> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceCastePayload, CasteModel>(url, payload)).data;
}

export async function readCaste(id: string): Promise<CasteModel> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<CasteModel>(url)).data;
}

export async function replaceCaste(id: string, payload: CreateOrReplaceCastePayload, version?: number): Promise<CasteModel> {
  const url: string = createUrlBuilder(id)
    .setQuery("version", version?.toString() ?? "")
    .buildRelative();
  return (await put<CreateOrReplaceCastePayload, CasteModel>(url, payload)).data;
}

export async function searchCastes(payload: SearchCastesPayload): Promise<SearchResults<CasteModel>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery(
      "search",
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
