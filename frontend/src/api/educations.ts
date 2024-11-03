import { urlUtils } from "logitar-js";

import type { CreateOrReplaceEducationPayload, EducationModel, SearchEducationsPayload } from "@/types/educations";
import type { SearchResults } from "@/types/search";
import { get, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/educations/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/educations" });
}

export async function createEducation(payload: CreateOrReplaceEducationPayload): Promise<EducationModel> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceEducationPayload, EducationModel>(url, payload)).data;
}

export async function readEducation(id: string): Promise<EducationModel> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<EducationModel>(url)).data;
}

export async function replaceEducation(id: string, payload: CreateOrReplaceEducationPayload, version?: number): Promise<EducationModel> {
  const url: string = createUrlBuilder(id)
    .setQuery("version", version?.toString() ?? "")
    .buildRelative();
  return (await put<CreateOrReplaceEducationPayload, EducationModel>(url, payload)).data;
}

export async function searchEducations(payload: SearchEducationsPayload): Promise<SearchResults<EducationModel>> {
  const url: string = createUrlBuilder()
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
  return (await get<SearchResults<EducationModel>>(url)).data;
}
