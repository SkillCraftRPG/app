import { urlUtils } from "logitar-js";

import type { Character, CreateCharacterPayload, SearchCharactersPayload } from "@/types/characters";
import type { SearchResults } from "@/types/search";
import { get, post } from ".";

export async function createCharacter(payload: CreateCharacterPayload): Promise<Character> {
  const url: string = new urlUtils.UrlBuilder({ path: "/characters" }).buildRelative();
  return (await post<CreateCharacterPayload, Character>(url, payload)).data;
}

export async function readCharacter(id: string): Promise<Character> {
  const url: string = new urlUtils.UrlBuilder({ path: "/characters/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Character>(url)).data;
}

export async function searchCharacters(payload: SearchCharactersPayload): Promise<SearchResults<Character>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/characters" })
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
  return (await get<SearchResults<Character>>(url)).data;
}
