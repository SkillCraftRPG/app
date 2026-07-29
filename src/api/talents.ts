import { urlUtils } from "logitar-js";

import type { CreateOrReplaceTalentPayload, SearchTalentsPayload, UpdateTalentPayload, Talent } from "@/types/talents";
import type { SearchResults } from "@/types/search";
import { get, patch, post, put } from ".";

export async function createTalent(payload: CreateOrReplaceTalentPayload): Promise<Talent> {
  const url: string = new urlUtils.UrlBuilder({ path: "/talents" }).buildRelative();
  return (await post<CreateOrReplaceTalentPayload, Talent>(url, payload)).data;
}

export async function readTalent(id: string): Promise<Talent> {
  const url: string = new urlUtils.UrlBuilder({ path: "/talents/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Talent>(url)).data;
}

export async function replaceTalent(id: string, payload: CreateOrReplaceTalentPayload): Promise<Talent> {
  const url: string = new urlUtils.UrlBuilder({ path: "/talents/{id}" }).setParameter("id", id).buildRelative();
  return (await put<CreateOrReplaceTalentPayload, Talent>(url, payload)).data;
}

export async function searchTalents(payload: SearchTalentsPayload): Promise<SearchResults<Talent>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/talents" })
    .setQuery("ids", payload.ids)
    .setQuery("multiple", payload.allowMultiplePurchases?.toString() ?? "")
    .setQuery("required", payload.requiredTalentId ?? "")
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery("skill", payload.skill ?? "")
    .setQuery(
      "tier",
      (payload.tiers ?? []).map((tier) => tier.toString()),
    )
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<Talent>>(url)).data;
}

export async function updateTalent(id: string, payload: UpdateTalentPayload): Promise<Talent> {
  const url: string = new urlUtils.UrlBuilder({ path: "/talents/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateTalentPayload, Talent>(url, payload)).data;
}
