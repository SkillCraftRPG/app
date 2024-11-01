import { urlUtils } from "logitar-js";

import type { CustomizationModel, SearchCustomizationsPayload } from "@/types/customizations";
import type { SearchResults } from "@/types/search";
import { get } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/customizations/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/customizations" });
}

export async function searchCustomizations(payload: SearchCustomizationsPayload): Promise<SearchResults<CustomizationModel>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery(
      "search_terms",
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
