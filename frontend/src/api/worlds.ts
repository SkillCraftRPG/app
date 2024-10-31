import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { SearchWorldsPayload, WorldModel } from "@/types/worlds";
import { get } from ".";

function createUrlBuilder(slug?: string): urlUtils.IUrlBuilder {
  return slug ? new urlUtils.UrlBuilder({ path: "/worlds/slug:{slug}" }).setParameter("slug", slug) : new urlUtils.UrlBuilder({ path: "/worlds" });
}

export async function readWorld(slug: string): Promise<WorldModel> {
  const url: string = createUrlBuilder(slug).buildRelative();
  return (await get<WorldModel>(url)).data;
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
