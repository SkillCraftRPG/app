import { urlUtils } from "logitar-js";

import type { CreateOrReplaceWorldPayload, SearchWorldsPayload, UpdateWorldPayload, World } from "@/types/worlds";
import type { SearchResults } from "@/types/search";
import { get, patch, post, put } from ".";

export async function createWorld(payload: CreateOrReplaceWorldPayload): Promise<World> {
  const url: string = new urlUtils.UrlBuilder({ path: "/worlds" }).buildRelative();
  return (await post<CreateOrReplaceWorldPayload, World>(url, payload)).data;
}

export async function readWorld(id: string): Promise<World> {
  const url: string = new urlUtils.UrlBuilder({ path: "/worlds/{id}" }).setParameter("id", id).buildRelative();
  return (await get<World>(url)).data;
}

export async function replaceWorld(id: string, payload: CreateOrReplaceWorldPayload): Promise<World> {
  const url: string = new urlUtils.UrlBuilder({ path: "/worlds/{id}" }).setParameter("id", id).buildRelative();
  return (await put<CreateOrReplaceWorldPayload, World>(url, payload)).data;
}

export async function searchWorlds(payload: SearchWorldsPayload): Promise<SearchResults<World>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/worlds" })
    .setQuery("ids", payload.ids)
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
  return (await get<SearchResults<World>>(url)).data;
}

export async function updateWorld(id: string, payload: UpdateWorldPayload): Promise<World> {
  const url: string = new urlUtils.UrlBuilder({ path: "/worlds/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateWorldPayload, World>(url, payload)).data;
}
