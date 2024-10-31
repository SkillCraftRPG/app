import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { SearchWorldsPayload, WorldModel } from "@/types/worlds";
import { get } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/worlds/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/worlds" });
}

export async function searchWorlds(payload: SearchWorldsPayload): Promise<SearchResults<WorldModel>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery(
      "search_terms",
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
  return (await get<SearchResults<WorldModel>>(url)).data;
}
