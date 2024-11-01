import { urlUtils } from "logitar-js";

import type { ItemModel, SearchItemsPayload } from "@/types/items";
import type { SearchResults } from "@/types/search";
import { get } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/items/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/items" });
}

export async function searchItems(payload: SearchItemsPayload): Promise<SearchResults<ItemModel>> {
  const builder: urlUtils.IUrlBuilder = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery(
      "search_terms",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery("attunement", payload.isAttunementRequired?.toString() ?? "")
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString());
  if (payload.value) {
    builder.setQuery("value_operator", payload.value?.operator ?? "");
    payload.value.values.forEach((value) => builder.addQuery("values", value.toString()));
  }
  if (payload.weight) {
    builder.setQuery("weight_operator", payload.weight?.operator ?? "");
    payload.weight.values.forEach((weight) => builder.addQuery("weights", weight.toString()));
  }
  const url: string = builder.buildRelative();
  return (await get<SearchResults<ItemModel>>(url)).data;
}
