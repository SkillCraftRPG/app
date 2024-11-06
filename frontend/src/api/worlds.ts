import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { CreateOrReplaceWorldPayload, SearchWorldsPayload, WorldModel } from "@/types/worlds";
import { get, post, put } from ".";

type UrlOptions = {
  id?: string;
  slug?: string;
};

function createUrlBuilder(options?: UrlOptions): urlUtils.IUrlBuilder {
  options ??= {};
  if (options.id) {
    return new urlUtils.UrlBuilder({ path: "/worlds/{id}" }).setParameter("id", options.id);
  } else if (options.slug) {
    return new urlUtils.UrlBuilder({ path: "/worlds/slug:{slug}" }).setParameter("slug", options.slug);
  }
  return new urlUtils.UrlBuilder({ path: "/worlds" });
}

export async function createWorld(payload: CreateOrReplaceWorldPayload): Promise<WorldModel> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceWorldPayload, WorldModel>(url, payload)).data;
}

export async function readWorld(slug: string): Promise<WorldModel> {
  const url: string = createUrlBuilder({ slug }).buildRelative();
  return (await get<WorldModel>(url)).data;
}

export async function replaceWorld(id: string, payload: CreateOrReplaceWorldPayload, version?: number): Promise<WorldModel> {
  const url: string = createUrlBuilder({ id })
    .setQuery("version", version?.toString() ?? "")
    .buildRelative();
  return (await put<CreateOrReplaceWorldPayload, WorldModel>(url, payload)).data;
}

export async function searchWorlds(payload: SearchWorldsPayload): Promise<SearchResults<WorldModel>> {
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
  return (await get<SearchResults<WorldModel>>(url)).data;
}
