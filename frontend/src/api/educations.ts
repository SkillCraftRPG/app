import { urlUtils } from "logitar-js";

import type { CreateOrReplaceEducationPayload, EducationModel, SearchEducationsPayload } from "@/types/educations";
import type { SearchResults } from "@/types/search";
import { get, post } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/educations/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/educations" });
}

export async function createEducation(payload: CreateOrReplaceEducationPayload): Promise<EducationModel> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceEducationPayload, EducationModel>(url, payload)).data;
}

export async function searchEducations(payload: SearchEducationsPayload): Promise<SearchResults<EducationModel>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery(
      "search_terms",
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
