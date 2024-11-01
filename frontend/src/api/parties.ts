import { urlUtils } from "logitar-js";

import type { PartyModel, SearchPartiesPayload } from "@/types/parties";
import type { SearchResults } from "@/types/search";
import { get } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/parties/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/parties" });
}

export async function searchParties(payload: SearchPartiesPayload): Promise<SearchResults<PartyModel>> {
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
  return (await get<SearchResults<PartyModel>>(url)).data;
}
