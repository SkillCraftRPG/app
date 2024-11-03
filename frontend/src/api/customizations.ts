import { urlUtils } from "logitar-js";

import type { CreateOrReplaceCustomizationPayload, CustomizationModel, SearchCustomizationsPayload } from "@/types/customizations";
import type { SearchResults } from "@/types/search";
import { get, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/customizations/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/customizations" });
}

export async function createCustomization(payload: CreateOrReplaceCustomizationPayload): Promise<CustomizationModel> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceCustomizationPayload, CustomizationModel>(url, payload)).data;
}

export async function readCustomization(id: string): Promise<CustomizationModel> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<CustomizationModel>(url)).data;
}

export async function replaceCustomization(id: string, payload: CreateOrReplaceCustomizationPayload, version?: number): Promise<CustomizationModel> {
  const url: string = createUrlBuilder(id)
    .setQuery("version", version?.toString() ?? "")
    .buildRelative();
  return (await put<CreateOrReplaceCustomizationPayload, CustomizationModel>(url, payload)).data;
}

export async function searchCustomizations(payload: SearchCustomizationsPayload): Promise<SearchResults<CustomizationModel>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery("type", payload.type ?? "")
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<CustomizationModel>>(url)).data;
}
