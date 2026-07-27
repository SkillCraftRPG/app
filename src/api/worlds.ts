import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { SearchWorldsPayload, World } from "@/types/worlds";
import { get } from ".";

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
