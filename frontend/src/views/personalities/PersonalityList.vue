<script setup lang="ts">
import { TarButton, parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import AppPagination from "@/components/shared/AppPagination.vue";
import AttributeSelect from "@/components/game/AttributeSelect.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreatePersonality from "@/components/personalities/CreatePersonality.vue";
import CustomizationSelect from "@/components/customizations/CustomizationSelect.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Attribute } from "@/types/game";
import type { PersonalityModel, PersonalitySort, SearchPersonalitiesPayload } from "@/types/personalities";
import { handleErrorKey } from "@/inject/App";
import { searchPersonalities } from "@/api/personalities";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const personalities = ref<PersonalityModel[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const attribute = computed<Attribute>(() => (route.query.attribute?.toString() as Attribute) ?? "");
const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const giftId = computed<string>(() => route.query.gift?.toString() ?? "");
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  arrayUtils.orderBy(
    Object.entries(tm(rt("personalities.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

function onCreated(personality: PersonalityModel): void {
  toasts.success("personalities.created");
  router.push({ name: "PersonalityEdit", params: { id: personality.id } });
}

async function refresh(): Promise<void> {
  const payload: SearchPersonalitiesPayload = {
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    attribute: attribute.value,
    giftId: giftId.value,
    sort: sort.value ? [{ field: sort.value as PersonalitySort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results = await searchPersonalities(payload);
    if (now === timestamp.value) {
      personalities.value = results.items;
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
    case "attribute":
    case "gift":
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
    if (route.name === "PersonalityList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: objectUtils.isEmpty(query)
            ? {
                attribute: "",
                gift: "",
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
    <h1>{{ t("personalities.list") }}</h1>
    <AppBreadcrumb :current="t('personalities.list')" @error="handleError" />
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
      <CreatePersonality class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="row">
      <AttributeSelect class="col-lg-6" :model-value="attribute" validation="server" @update:model-value="setQuery('attribute', $event ?? '')" />
      <CustomizationSelect
        class="col-lg-6"
        label="customizations.type.options.Gift"
        :model-value="giftId"
        placeholder="customizations.select.gift"
        type="Gift"
        validation="server"
        @error="handleError"
        @update:model-value="setQuery('gift', $event ?? '')"
      />
    </div>
    <div class="row">
      <SearchInput class="col-lg-4" :model-value="search" @update:model-value="setQuery('search', $event ?? '')" />
      <SortSelect
        class="col-lg-4"
        :descending="isDescending"
        :model-value="sort"
        :options="sortOptions"
        @descending="setQuery('isDescending', $event.toString())"
        @update:model-value="setQuery('sort', $event ?? '')"
      />
      <CountSelect class="col-lg-4" :model-value="count" @update:model-value="setQuery('count', ($event ?? 10).toString())" />
    </div>
    <template v-if="personalities.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("personalities.sort.options.Name") }}</th>
            <th scope="col">{{ t("game.attribute") }}</th>
            <th scope="col">{{ t("customizations.type.options.Gift") }}</th>
            <th scope="col">{{ t("personalities.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="personality in personalities" :key="personality.id">
            <td>
              <RouterLink :to="{ name: 'PersonalityEdit', params: { id: personality.id } }">
                <font-awesome-icon icon="fas fa-edit" />{{ personality.name }}
              </RouterLink>
            </td>
            <td>{{ personality.attribute ? t(`game.attributes.${personality.attribute}`) : "—" }}</td>
            <td>
              <RouterLink v-if="personality.gift" :to="{ name: 'CustomizationEdit', params: { id: personality.gift.id } }" target="_blank">
                <font-awesome-icon icon="fas fa-eye" />{{ personality.gift.name }}
              </RouterLink>
              <template v-else>{{ "—" }}</template>
            </td>
            <td><StatusBlock :actor="personality.updatedBy" :date="personality.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("personalities.empty") }}</p>
  </main>
</template>
