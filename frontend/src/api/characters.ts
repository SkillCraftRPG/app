import { urlUtils } from "logitar-js";

import type { CharacterModel, CreateCharacterPayload, SearchCharactersPayload } from "@/types/characters";
import type { SearchResults } from "@/types/search";
import { get, post } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/characters/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/characters" });
}

export async function createCharacter(payload: CreateCharacterPayload): Promise<CharacterModel> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateCharacterPayload, CharacterModel>(url, payload)).data;
}

export async function readCharacter(id: string): Promise<CharacterModel> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<CharacterModel>(url)).data;
}

export async function searchCharacters(payload: SearchCharactersPayload): Promise<SearchResults<CharacterModel>> {
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
  return (await get<SearchResults<CharacterModel>>(url)).data;
}
