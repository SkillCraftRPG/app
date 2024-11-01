import { urlUtils } from "logitar-js";

import type { CreateOrReplacePersonalityPayload, PersonalityModel, SearchPersonalitiesPayload } from "@/types/personalities";
import type { SearchResults } from "@/types/search";
import { get, post } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/personalities/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/personalities" });
}

export async function createPersonality(payload: CreateOrReplacePersonalityPayload): Promise<PersonalityModel> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplacePersonalityPayload, PersonalityModel>(url, payload)).data;
}

export async function searchPersonalities(payload: SearchPersonalitiesPayload): Promise<SearchResults<PersonalityModel>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery(
      "search_terms",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery("attribute", payload.attribute ?? "")
    .setQuery("gift", payload.giftId ?? "")
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<PersonalityModel>>(url)).data;
}
