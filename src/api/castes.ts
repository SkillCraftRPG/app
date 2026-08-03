import { urlUtils } from "logitar-js";

import type { CreateOrReplaceCastePayload, SearchCastesPayload, UpdateCastePayload, Caste } from "@/types/castes";
import type { SearchResults } from "@/types/search";
import { get, patch, post, put } from ".";

export async function createCaste(payload: CreateOrReplaceCastePayload): Promise<Caste> {
  const url: string = new urlUtils.UrlBuilder({ path: "/castes" }).buildRelative();
  return (await post<CreateOrReplaceCastePayload, Caste>(url, payload)).data;
}

export async function listCastes(): Promise<SearchResults<Caste>> {
  const payload: SearchCastesPayload = {
    ids: [],
    search: { terms: [], operator: "And" },
    sort: [],
    skip: 0,
    limit: 0,
  };
  return await searchCastes(payload);
}

export async function readCaste(id: string): Promise<Caste> {
  const url: string = new urlUtils.UrlBuilder({ path: "/castes/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Caste>(url)).data;
}

export async function replaceCaste(id: string, payload: CreateOrReplaceCastePayload): Promise<Caste> {
  const url: string = new urlUtils.UrlBuilder({ path: "/castes/{id}" }).setParameter("id", id).buildRelative();
  return (await put<CreateOrReplaceCastePayload, Caste>(url, payload)).data;
}

export async function saveCaste(caste: Caste): Promise<Caste> {
  const payload: CreateOrReplaceCastePayload = {
    name: caste.name,
    summary: caste.summary,
    content: caste.content,
    skill: caste.skill,
    wealthRoll: caste.wealthRoll,
    feature: caste.feature,
  };
  return await replaceCaste(caste.id, payload);
}

export async function searchCastes(payload: SearchCastesPayload): Promise<SearchResults<Caste>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/castes" })
    .setQuery("ids", payload.ids)
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery("skill", payload.skill ?? "")
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<Caste>>(url)).data;
}

export async function updateCaste(id: string, payload: UpdateCastePayload): Promise<Caste> {
  const url: string = new urlUtils.UrlBuilder({ path: "/castes/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateCastePayload, Caste>(url, payload)).data;
}
