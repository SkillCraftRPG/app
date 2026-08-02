import { urlUtils } from "logitar-js";

import type { CreateOrReplaceLineagePayload, Lineage, SearchLineagesPayload, UpdateLineagePayload } from "@/types/lineages";
import type { Feature } from "@/types/features";
import type { SearchResults } from "@/types/search";
import { _delete, get, patch, post, put } from ".";

export async function createLineage(payload: CreateOrReplaceLineagePayload): Promise<Lineage> {
  const url: string = new urlUtils.UrlBuilder({ path: "/lineages" }).buildRelative();
  return (await post<CreateOrReplaceLineagePayload, Lineage>(url, payload)).data;
}

export async function readLineage(id: string): Promise<Lineage> {
  const url: string = new urlUtils.UrlBuilder({ path: "/lineages/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Lineage>(url)).data;
}

export async function replaceLineage(id: string, payload: CreateOrReplaceLineagePayload): Promise<Lineage> {
  const url: string = new urlUtils.UrlBuilder({ path: "/lineages/{id}" }).setParameter("id", id).buildRelative();
  return (await put<CreateOrReplaceLineagePayload, Lineage>(url, payload)).data;
}

export async function replaceLineageFeature(lineageId: string, featureId: string, payload: Feature): Promise<Lineage> {
  const url: string = new urlUtils.UrlBuilder({ path: "/lineages/{lineageId}/features/{featureId}" })
    .setParameter("lineageId", lineageId)
    .setParameter("featureId", featureId)
    .buildRelative();
  return (await put<Feature, Lineage>(url, payload)).data;
}

export async function searchLineages(payload: SearchLineagesPayload): Promise<SearchResults<Lineage>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/lineages" })
    .setQuery("ids", payload.ids)
    .setQuery("parent", payload.parentId ?? "")
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery("size", payload.sizeCategory ?? "")
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<Lineage>>(url)).data;
}

export async function updateLineage(id: string, payload: UpdateLineagePayload): Promise<Lineage> {
  const url: string = new urlUtils.UrlBuilder({ path: "/lineages/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateLineagePayload, Lineage>(url, payload)).data;
}
