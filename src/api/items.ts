import { urlUtils } from "logitar-js";

import type { CreateOrReplaceItemPayload, SearchItemsPayload, UpdateItemPayload, Item } from "@/types/items";
import type { SearchResults } from "@/types/search";
import { get, patch, post, put } from ".";

export async function createItem(payload: CreateOrReplaceItemPayload): Promise<Item> {
  const url: string = new urlUtils.UrlBuilder({ path: "/items" }).buildRelative();
  return (await post<CreateOrReplaceItemPayload, Item>(url, payload)).data;
}

export async function listItems(): Promise<SearchResults<Item>> {
  const payload: SearchItemsPayload = {
    ids: [],
    search: { terms: [], operator: "And" },
    sort: [],
    skip: 0,
    limit: 0,
  };
  return await searchItems(payload);
}

export async function readItem(id: string): Promise<Item> {
  const url: string = new urlUtils.UrlBuilder({ path: "/items/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Item>(url)).data;
}

export async function replaceItem(id: string, payload: CreateOrReplaceItemPayload): Promise<Item> {
  const url: string = new urlUtils.UrlBuilder({ path: "/items/{id}" }).setParameter("id", id).buildRelative();
  return (await put<CreateOrReplaceItemPayload, Item>(url, payload)).data;
}

export async function searchItems(payload: SearchItemsPayload): Promise<SearchResults<Item>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/items" })
    .setQuery("category", payload.category ?? "")
    .setQuery("ids", payload.ids)
    .setQuery("rarity", payload.rarity ?? "")
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
  return (await get<SearchResults<Item>>(url)).data;
}

export async function updateItem(id: string, payload: UpdateItemPayload): Promise<Item> {
  const url: string = new urlUtils.UrlBuilder({ path: "/items/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateItemPayload, Item>(url, payload)).data;
}
