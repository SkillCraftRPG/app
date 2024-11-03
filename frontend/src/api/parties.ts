import { urlUtils } from "logitar-js";

import type { CreateOrReplacePartyPayload, PartyModel, SearchPartiesPayload } from "@/types/parties";
import type { SearchResults } from "@/types/search";
import { get, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/parties/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/parties" });
}

export async function createParty(payload: CreateOrReplacePartyPayload): Promise<PartyModel> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplacePartyPayload, PartyModel>(url, payload)).data;
}

export async function readParty(id: string): Promise<PartyModel> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<PartyModel>(url)).data;
}

export async function replaceParty(id: string, payload: CreateOrReplacePartyPayload, version?: number): Promise<PartyModel> {
  const url: string = createUrlBuilder(id)
    .setQuery("version", version?.toString() ?? "")
    .buildRelative();
  return (await put<CreateOrReplacePartyPayload, PartyModel>(url, payload)).data;
}

export async function searchParties(payload: SearchPartiesPayload): Promise<SearchResults<PartyModel>> {
  const url: string = createUrlBuilder()
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
  return (await get<SearchResults<PartyModel>>(url)).data;
}
