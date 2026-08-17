import { urlUtils } from "logitar-js";

import type {
  Character,
  CreateCharacterPayload,
  CreateOrReplaceCharacterModifierPayload,
  SearchCharactersPayload,
  UpdateCharacterPayload,
} from "@/types/characters";
import type { SearchResults } from "@/types/search";
import { _delete, get, patch, post, put } from ".";

export async function createCharacter(payload: CreateCharacterPayload): Promise<Character> {
  const url: string = new urlUtils.UrlBuilder({ path: "/characters" }).buildRelative();
  return (await post<CreateCharacterPayload, Character>(url, payload)).data;
}

export async function createCharacterModifier(characterId: string, payload: CreateOrReplaceCharacterModifierPayload): Promise<Character> {
  const url: string = new urlUtils.UrlBuilder({ path: "/characters/{characterId}/modifiers" }).setParameter("characterId", characterId).buildRelative();
  return (await post<CreateOrReplaceCharacterModifierPayload, Character>(url, payload)).data;
}

export async function readCharacter(id: string): Promise<Character> {
  const url: string = new urlUtils.UrlBuilder({ path: "/characters/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Character>(url)).data;
}

export async function removeCharacterModifier(characterId: string, modifierId: string): Promise<Character> {
  const url: string = new urlUtils.UrlBuilder({ path: "/characters/{characterId}/modifiers/{modifierId}" })
    .setParameter("characterId", characterId)
    .setParameter("modifierId", modifierId)
    .buildRelative();
  return (await _delete<Character>(url)).data;
}

export async function replaceCharacterModifier(characterId: string, modifierId: string, payload: CreateOrReplaceCharacterModifierPayload): Promise<Character> {
  const url: string = new urlUtils.UrlBuilder({ path: "/characters/{characterId}/modifiers/{modifierId}" })
    .setParameter("characterId", characterId)
    .setParameter("modifierId", modifierId)
    .buildRelative();
  return (await put<CreateOrReplaceCharacterModifierPayload, Character>(url, payload)).data;
}

export async function searchCharacters(payload: SearchCharactersPayload): Promise<SearchResults<Character>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/characters" })
    .setQuery("caste", payload.casteId ?? "")
    .setQuery("education", payload.educationId ?? "")
    .setQuery("ids", payload.ids)
    .setQuery("lineage", payload.lineageId ?? "")
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

export async function updateCharacter(id: string, payload: UpdateCharacterPayload): Promise<Character> {
  const url: string = new urlUtils.UrlBuilder({ path: "/characters/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateCharacterPayload, Character>(url, payload)).data;
}
