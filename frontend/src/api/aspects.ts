import { urlUtils } from "logitar-js";

import type { AspectModel, SearchAspectsPayload } from "@/types/aspects";
import type { SearchResults } from "@/types/search";
import { get } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/aspects/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/aspects" });
}

export async function searchAspects(payload: SearchAspectsPayload): Promise<SearchResults<AspectModel>> {
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
  return (await get<SearchResults<AspectModel>>(url)).data;
}
