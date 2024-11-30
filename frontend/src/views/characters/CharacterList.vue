<script setup lang="ts">
import { TarButton, parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import AppPagination from "@/components/shared/AppPagination.vue";
import CasteIcon from "@/components/castes/CasteIcon.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import EducationIcon from "@/components/educations/EducationIcon.vue";
import LineageLink from "@/components/lineages/LineageLink.vue";
import NatureLink from "@/components/natures/NatureLink.vue";
import PlayerSelect from "@/components/characters/PlayerSelect.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { CharacterModel, CharacterSort, SearchCharactersPayload } from "@/types/characters";
import { handleErrorKey } from "@/inject/App";
import { searchCharacters } from "@/api/characters";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const characters = ref<CharacterModel[]>([]);
const isLoading = ref<boolean>(false);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const player = computed<string>(() => route.query.player?.toString() ?? "");
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  arrayUtils.orderBy(
    Object.entries(tm(rt("characters.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchCharactersPayload = {
    ids: [],
    playerName: player.value,
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as CharacterSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results = await searchCharacters(payload);
    if (now === timestamp.value) {
      characters.value = results.items;
      total.value = results.total;
    }
  } catch (e: unknown) {
    handleError(e);
  } finally {
    if (now === timestamp.value) {
      isLoading.value = false;
    }
  }
}

function setQuery(key: string, value: string): void {
  const query = { ...route.query, [key]: value };
  switch (key) {
    case "player":
    case "search":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

watch(
  () => route,
  (route) => {
    if (route.name === "CharacterList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: objectUtils.isEmpty(query)
            ? {
                player: "",
                search: "",
                sort: "Name",
                isDescending: "false",
                page: 1,
                count: 10,
              }
            : {
                page: 1,
                count: 10,
                ...query,
              },
        });
      } else {
        refresh();
      }
    }
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <main class="container">
    <h1>{{ t("characters.list") }}</h1>
    <AppBreadcrumb :current="t('characters.list')" @error="handleError" />
    <div class="my-3">
      <TarButton
        class="me-1"
        :disabled="isLoading"
        icon="fas fa-rotate"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('actions.refresh')"
        @click="refresh()"
      />
      <RouterLink :to="{ name: 'CharacterCreation' }" class="btn btn-success ms-1">
        <font-awesome-icon icon="fas fa-plus" /> {{ t("actions.create") }}
      </RouterLink>
    </div>
    <div class="row">
      <PlayerSelect class="col-lg-3" :model-value="player" @update:model-value="setQuery('player', $event ?? '')" />
      <SearchInput class="col-lg-3" :model-value="search" @update:model-value="setQuery('search', $event ?? '')" />
      <SortSelect
        class="col-lg-3"
        :descending="isDescending"
        :model-value="sort"
        :options="sortOptions"
        @descending="setQuery('isDescending', $event.toString())"
        @update:model-value="setQuery('sort', $event ?? '')"
      />
      <CountSelect class="col-lg-3" :model-value="count" @update:model-value="setQuery('count', ($event ?? 10).toString())" />
    </div>
    <template v-if="characters.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("characters.name") }}</th>
            <th scope="col">{{ t("characters.progression") }}</th>
            <th scope="col">{{ t("characters.lineage") }}</th>
            <th scope="col">{{ t("characters.background.label") }}</th>
            <th scope="col">{{ t("natures.select.label") }}</th>
            <th scope="col">{{ t("characters.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="character in characters" :key="character.id">
            <td>
              <RouterLink :to="{ name: 'CharacterEdit', params: { id: character.id } }">
                <font-awesome-icon icon="fas fa-edit" />{{ character.name }}
              </RouterLink>
              <template v-if="character.playerName">
                <br />
                <font-awesome-icon icon="fas fa-user" /> {{ character.playerName }}
              </template>
            </td>
            <td>
              {{ t("characters.level.label") }} {{ character.level }}
              <br />
              {{ t("characters.tier") }} {{ character.tier }}
            </td>
            <td>
              <template v-if="character.lineage.species">
                <LineageLink :lineage="character.lineage.species" />
                <br />
              </template>
              <LineageLink :lineage="character.lineage" />
            </td>
            <td>
              <RouterLink :to="{ name: 'CasteEdit', params: { id: character.caste.id } }" target="_blank"> <CasteIcon />{{ character.caste.name }} </RouterLink>
              <br />
              <RouterLink :to="{ name: 'EducationEdit', params: { id: character.education.id } }" target="_blank">
                <EducationIcon />{{ character.education.name }}
              </RouterLink>
            </td>
            <td><NatureLink :nature="character.nature" /></td>
            <td><StatusBlock :actor="character.updatedBy" :date="character.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("characters.empty") }}</p>
  </main>
</template>
