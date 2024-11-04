import { urlUtils } from "logitar-js";

import type { CreateOrReplaceItemPayload, ItemModel, SearchItemsPayload } from "@/types/items";
import type { SearchResults } from "@/types/search";
import { get, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/items/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/items" });
}

export async function createItem(payload: CreateOrReplaceItemPayload): Promise<ItemModel> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceItemPayload, ItemModel>(url, payload)).data;
}

export async function readItem(id: string): Promise<ItemModel> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<ItemModel>(url)).data;
}

export async function replaceItem(id: string, payload: CreateOrReplaceItemPayload, version?: number): Promise<ItemModel> {
  const url: string = createUrlBuilder(id)
    .setQuery("version", version?.toString() ?? "")
    .buildRelative();
  return (await put<CreateOrReplaceItemPayload, ItemModel>(url, payload)).data;
}

export async function searchItems(payload: SearchItemsPayload): Promise<SearchResults<ItemModel>> {
  const builder: urlUtils.IUrlBuilder = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery("attunement", payload.isAttunementRequired?.toString() ?? "")
    .setQuery("category", payload.category ?? "")
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
