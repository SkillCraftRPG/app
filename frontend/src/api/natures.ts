import { urlUtils } from "logitar-js";

import type { CreateOrReplaceNaturePayload, NatureModel, SearchNaturesPayload } from "@/types/natures";
import type { SearchResults } from "@/types/search";
import { get, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/natures/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/natures" });
}

export async function createNature(payload: CreateOrReplaceNaturePayload): Promise<NatureModel> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceNaturePayload, NatureModel>(url, payload)).data;
}

export async function readNature(id: string): Promise<NatureModel> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<NatureModel>(url)).data;
}

export async function replaceNature(id: string, payload: CreateOrReplaceNaturePayload, version?: number): Promise<NatureModel> {
  const url: string = createUrlBuilder(id)
    .setQuery("version", version?.toString() ?? "")
    .buildRelative();
  return (await put<CreateOrReplaceNaturePayload, NatureModel>(url, payload)).data;
}

export async function searchNatures(payload: SearchNaturesPayload): Promise<SearchResults<NatureModel>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery("attribute", payload.attribute ?? "")
    .setQuery("gift", payload.giftId ?? "")
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<NatureModel>>(url)).data;
}
