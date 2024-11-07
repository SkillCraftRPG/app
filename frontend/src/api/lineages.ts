import { urlUtils } from "logitar-js";

import type { LineageModel, CreateOrReplaceLineagePayload, SearchLineagesPayload } from "@/types/lineages";
import type { SearchResults } from "@/types/search";
import { get, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/lineages/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/lineages" });
}

export async function createLineage(payload: CreateOrReplaceLineagePayload): Promise<LineageModel> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceLineagePayload, LineageModel>(url, payload)).data;
}

export async function readLineage(id: string): Promise<LineageModel> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<LineageModel>(url)).data;
}

export async function replaceLineage(id: string, payload: CreateOrReplaceLineagePayload, version?: number): Promise<LineageModel> {
  const url: string = createUrlBuilder(id)
    .setQuery("version", version?.toString() ?? "")
    .buildRelative();
  return (await put<CreateOrReplaceLineagePayload, LineageModel>(url, payload)).data;
}

export async function searchLineages(payload: SearchLineagesPayload): Promise<SearchResults<LineageModel>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery("attribute", payload.attribute ?? "")
    .setQuery("language", payload.languageId ?? "")
    .setQuery("parent", payload.parentId ?? "")
    .setQuery("size", payload.sizeCategory ?? "")
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<LineageModel>>(url)).data;
}
