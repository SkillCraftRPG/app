import { urlUtils } from "logitar-js";

import type { CreateOrReplaceTalentPayload, SearchTalentsPayload, TalentModel } from "@/types/talents";
import type { SearchResults } from "@/types/search";
import { get, post } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/talents/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/talents" });
}

export async function createTalent(payload: CreateOrReplaceTalentPayload): Promise<TalentModel> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceTalentPayload, TalentModel>(url, payload)).data;
}

export async function searchTalents(payload: SearchTalentsPayload): Promise<SearchResults<TalentModel>> {
  const builder: urlUtils.IUrlBuilder = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery("multiple", payload.allowMultiplePurchases?.toString() ?? "")
    .setQuery("required", payload.requiredTalentId ?? "")
    .setQuery("skill", payload.hasSkill?.toString() ?? "")
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString());
  if (payload.tier) {
    builder.setQuery("tier_operator", payload.tier?.operator ?? "");
    payload.tier.values.forEach((tier) => builder.addQuery("tiers", tier.toString()));
  }
  const url: string = builder.buildRelative();
  return (await get<SearchResults<TalentModel>>(url)).data;
}
