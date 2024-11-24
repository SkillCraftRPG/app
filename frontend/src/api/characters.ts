import { urlUtils } from "logitar-js";

import type {
  BonusPayload,
  CharacterLanguagePayload,
  CharacterModel,
  CharacterTalentPayload,
  CreateCharacterPayload,
  ReplaceCharacterPayload,
  SearchCharactersPayload,
} from "@/types/characters";
import type { SearchResults } from "@/types/search";
import { _delete, get, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/characters/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/characters" });
}

export async function addBonus(characterId: string, payload: BonusPayload): Promise<CharacterModel> {
  const url: string = new urlUtils.UrlBuilder().setPath("/characters/{characterId}/bonuses").setParameter("characterId", characterId).buildRelative();
  return (await post<BonusPayload, CharacterModel>(url, payload)).data;
}

export async function addCharacterTalent(characterId: string, payload: CharacterTalentPayload): Promise<CharacterModel> {
  const url: string = new urlUtils.UrlBuilder().setPath("/characters/{characterId}/talents").setParameter("characterId", characterId).buildRelative();
  return (await post<CharacterTalentPayload, CharacterModel>(url, payload)).data;
}

export async function createCharacter(payload: CreateCharacterPayload): Promise<CharacterModel> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateCharacterPayload, CharacterModel>(url, payload)).data;
}

export async function listPlayers(): Promise<SearchResults<string>> {
  const url: string = createUrlBuilder("players").buildRelative();
  return (await get<SearchResults<string>>(url)).data;
}

export async function readCharacter(id: string): Promise<CharacterModel> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<CharacterModel>(url)).data;
}

export async function removeBonus(characterId: string, bonusId: string): Promise<CharacterModel> {
  const url: string = new urlUtils.UrlBuilder({ path: "/characters/{characterId}/bonuses/{bonusId}" })
    .setParameter("characterId", characterId)
    .setParameter("bonusId", bonusId)
    .buildRelative();
  return (await _delete<CharacterModel>(url)).data;
}

export async function removeCharacterLanguage(characterId: string, languageId: string): Promise<CharacterModel> {
  const url: string = new urlUtils.UrlBuilder({ path: "/characters/{characterId}/languages/{languageId}" })
    .setParameter("characterId", characterId)
    .setParameter("languageId", languageId)
    .buildRelative();
  return (await _delete<CharacterModel>(url)).data;
}

export async function removeCharacterTalent(characterId: string, talentId: string): Promise<CharacterModel> {
  const url: string = new urlUtils.UrlBuilder({ path: "/characters/{characterId}/bonuses/{talentId}" })
    .setParameter("characterId", characterId)
    .setParameter("talentId", talentId)
    .buildRelative();
  return (await _delete<CharacterModel>(url)).data;
}

export async function replaceCharacter(id: string, payload: ReplaceCharacterPayload, version?: number): Promise<CharacterModel> {
  const url: string = createUrlBuilder(id)
    .setQuery("version", version?.toString() ?? "")
    .buildRelative();
  return (await put<ReplaceCharacterPayload, CharacterModel>(url, payload)).data;
}

export async function saveBonus(characterId: string, bonusId: string, payload: BonusPayload): Promise<CharacterModel> {
  const url: string = new urlUtils.UrlBuilder()
    .setPath("/characters/{characterId}/bonuses/{bonusId}")
    .setParameter("characterId", characterId)
    .setParameter("bonusId", bonusId)
    .buildRelative();
  return (await put<BonusPayload, CharacterModel>(url, payload)).data;
}

export async function saveCharacterLanguage(characterId: string, languageId: string, payload: CharacterLanguagePayload): Promise<CharacterModel> {
  const url: string = new urlUtils.UrlBuilder({ path: "/characters/{characterId}/languages/{languageId}" })
    .setParameter("characterId", characterId)
    .setParameter("languageId", languageId)
    .buildRelative();
  return (await put<CharacterLanguagePayload, CharacterModel>(url, payload)).data;
}

export async function saveCharacterTalent(characterId: string, talentId: string, payload: CharacterTalentPayload): Promise<CharacterModel> {
  const url: string = new urlUtils.UrlBuilder()
    .setPath("/characters/{characterId}/talents/{talentId}")
    .setParameter("characterId", characterId)
    .setParameter("talentId", talentId)
    .buildRelative();
  return (await put<CharacterTalentPayload, CharacterModel>(url, payload)).data;
}

export async function searchCharacters(payload: SearchCharactersPayload): Promise<SearchResults<CharacterModel>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery("player", payload.playerName ?? "")
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
