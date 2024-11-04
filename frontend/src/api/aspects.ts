import { urlUtils } from "logitar-js";

import type { AspectModel, CreateOrReplaceAspectPayload, SearchAspectsPayload } from "@/types/aspects";
import type { SearchResults } from "@/types/search";
import { get, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/aspects/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/aspects" });
}

export async function createAspect(payload: CreateOrReplaceAspectPayload): Promise<AspectModel> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceAspectPayload, AspectModel>(url, payload)).data;
}

export async function readAspect(id: string): Promise<AspectModel> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<AspectModel>(url)).data;
}

export async function replaceAspect(id: string, payload: CreateOrReplaceAspectPayload, version?: number): Promise<AspectModel> {
  const url: string = createUrlBuilder(id)
    .setQuery("version", version?.toString() ?? "")
    .buildRelative();
  return (await put<CreateOrReplaceAspectPayload, AspectModel>(url, payload)).data;
}

export async function searchAspects(payload: SearchAspectsPayload): Promise<SearchResults<AspectModel>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery("attribute", payload.attribute ?? "")
    .setQuery("skill", payload.skill ?? "")
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<AspectModel>>(url)).data;
}
