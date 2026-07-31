import { urlUtils } from "logitar-js";

import type { CreateOrReplaceSpellPayload, SearchSpellsPayload, UpdateSpellPayload, Spell } from "@/types/spells";
import type { SearchResults } from "@/types/search";
import { get, patch, post, put } from ".";

export async function createSpell(payload: CreateOrReplaceSpellPayload): Promise<Spell> {
  const url: string = new urlUtils.UrlBuilder({ path: "/spells" }).buildRelative();
  return (await post<CreateOrReplaceSpellPayload, Spell>(url, payload)).data;
}

export async function readSpell(id: string): Promise<Spell> {
  const url: string = new urlUtils.UrlBuilder({ path: "/spells/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Spell>(url)).data;
}

export async function replaceSpell(id: string, payload: CreateOrReplaceSpellPayload): Promise<Spell> {
  const url: string = new urlUtils.UrlBuilder({ path: "/spells/{id}" }).setParameter("id", id).buildRelative();
  return (await put<CreateOrReplaceSpellPayload, Spell>(url, payload)).data;
}

export async function searchSpells(payload: SearchSpellsPayload): Promise<SearchResults<Spell>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/spells" })
    .setQuery("ids", payload.ids)
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
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
  return (await get<SearchResults<Spell>>(url)).data;
}

export async function updateSpell(id: string, payload: UpdateSpellPayload): Promise<Spell> {
  const url: string = new urlUtils.UrlBuilder({ path: "/spells/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateSpellPayload, Spell>(url, payload)).data;
}
