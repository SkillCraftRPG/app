import { urlUtils } from "logitar-js";

import type { CreateOrReplaceEducationPayload, SearchEducationsPayload, UpdateEducationPayload, Education } from "@/types/educations";
import type { SearchResults } from "@/types/search";
import { get, patch, post, put } from ".";

export async function createEducation(payload: CreateOrReplaceEducationPayload): Promise<Education> {
  const url: string = new urlUtils.UrlBuilder({ path: "/educations" }).buildRelative();
  return (await post<CreateOrReplaceEducationPayload, Education>(url, payload)).data;
}

export async function readEducation(id: string): Promise<Education> {
  const url: string = new urlUtils.UrlBuilder({ path: "/educations/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Education>(url)).data;
}

export async function replaceEducation(id: string, payload: CreateOrReplaceEducationPayload): Promise<Education> {
  const url: string = new urlUtils.UrlBuilder({ path: "/educations/{id}" }).setParameter("id", id).buildRelative();
  return (await put<CreateOrReplaceEducationPayload, Education>(url, payload)).data;
}

export async function searchEducations(payload: SearchEducationsPayload): Promise<SearchResults<Education>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/educations" })
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
  return (await get<SearchResults<Education>>(url)).data;
}

export async function updateEducation(id: string, payload: UpdateEducationPayload): Promise<Education> {
  const url: string = new urlUtils.UrlBuilder({ path: "/educations/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateEducationPayload, Education>(url, payload)).data;
}
