import { urlUtils } from "logitar-js";

import type { CreateOrReplaceCustomizationPayload, SearchCustomizationsPayload, UpdateCustomizationPayload, Customization } from "@/types/customizations";
import type { SearchResults } from "@/types/search";
import { get, patch, post, put } from ".";

export async function createCustomization(payload: CreateOrReplaceCustomizationPayload): Promise<Customization> {
  const url: string = new urlUtils.UrlBuilder({ path: "/customizations" }).buildRelative();
  return (await post<CreateOrReplaceCustomizationPayload, Customization>(url, payload)).data;
}

export async function readCustomization(id: string): Promise<Customization> {
  const url: string = new urlUtils.UrlBuilder({ path: "/customizations/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Customization>(url)).data;
}

export async function replaceCustomization(id: string, payload: CreateOrReplaceCustomizationPayload): Promise<Customization> {
  const url: string = new urlUtils.UrlBuilder({ path: "/customizations/{id}" }).setParameter("id", id).buildRelative();
  return (await put<CreateOrReplaceCustomizationPayload, Customization>(url, payload)).data;
}

export async function searchCustomizations(payload: SearchCustomizationsPayload): Promise<SearchResults<Customization>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/customizations" })
    .setQuery("ids", payload.ids)
    .setQuery("kind", payload.kind ?? "")
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
  return (await get<SearchResults<Customization>>(url)).data;
}

export async function updateCustomization(id: string, payload: UpdateCustomizationPayload): Promise<Customization> {
  const url: string = new urlUtils.UrlBuilder({ path: "/customizations/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateCustomizationPayload, Customization>(url, payload)).data;
}
