import { urlUtils } from "logitar-js";

import type { CreateOrReplaceScriptPayload, SearchScriptsPayload, UpdateScriptPayload, Script } from "@/types/scripts";
import type { SearchResults } from "@/types/search";
import { get, patch, post, put } from ".";

export async function createScript(payload: CreateOrReplaceScriptPayload): Promise<Script> {
  const url: string = new urlUtils.UrlBuilder({ path: "/scripts" }).buildRelative();
  return (await post<CreateOrReplaceScriptPayload, Script>(url, payload)).data;
}

export async function listScripts(): Promise<SearchResults<Script>> {
  const payload: SearchScriptsPayload = {
    ids: [],
    search: { terms: [], operator: "And" },
    sort: [],
    skip: 0,
    limit: 0,
  };
  return await searchScripts(payload);
}

export async function readScript(id: string): Promise<Script> {
  const url: string = new urlUtils.UrlBuilder({ path: "/scripts/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Script>(url)).data;
}

export async function replaceScript(id: string, payload: CreateOrReplaceScriptPayload): Promise<Script> {
  const url: string = new urlUtils.UrlBuilder({ path: "/scripts/{id}" }).setParameter("id", id).buildRelative();
  return (await put<CreateOrReplaceScriptPayload, Script>(url, payload)).data;
}

export async function saveScript(script: Script): Promise<Script> {
  const payload: CreateOrReplaceScriptPayload = {
    name: script.name,
    summary: script.summary,
    content: script.content,
  };
  return await replaceScript(script.id, payload);
}

export async function searchScripts(payload: SearchScriptsPayload): Promise<SearchResults<Script>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/scripts" })
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
  return (await get<SearchResults<Script>>(url)).data;
}

export async function updateScript(id: string, payload: UpdateScriptPayload): Promise<Script> {
  const url: string = new urlUtils.UrlBuilder({ path: "/scripts/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateScriptPayload, Script>(url, payload)).data;
}
