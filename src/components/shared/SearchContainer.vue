<template>
  <section class="row mb-3">
    <SearchInput class="col" v-model="search" />
    <SortSelect class="col" :descending="isDescending" :options="sortOptions" v-model="sort" @descending="isDescending = $event" />
    <CountSelect class="col" v-model="count" />
  </section>
  <section>
    <slot v-if="total"></slot>
    <div v-else class="d-flex flex-column align-items-center justify-content-center text-center flex-grow-1 py-5">
      <font-awesome-icon icon="fas fa-magnifying-glass" class="display-4 text-body-secondary mb-3" aria-hidden="true" />
      <h2 class="h4 mb-2">{{ t("empty.lead") }}</h2>
      <p class="text-body-secondary mb-0">{{ t("empty.help") }}</p>
    </div>
  </section>
  <section v-if="total">
    <SearchPagination :count="count" :total="total" v-model="page" />
  </section>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import CountSelect from "./CountSelect.vue";
import SearchInput from "./SearchInput.vue";
import SearchPagination from "./SearchPagination.vue";
import SortSelect from "./SortSelect.vue";
import type { SelectOption } from "@/types/tar/select";

const { t } = useI18n();

withDefaults(
  defineProps<{
    sortOptions?: SelectOption[];
  }>(),
  {
    sortOptions: () => [],
  },
);

const count = ref<number>(10);
const isDescending = ref<boolean>(true);
const page = ref<number>(1);
const search = ref<string>("");
const sort = ref<string>("UpdatedOn");
const total = ref<number>(0);

// TODO(fpion): we could vertically center the no-result section.
// TODO(fpion): we could have a clear/reset filters button into the no-result section.
</script>
