<script setup lang="ts">
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";

import type { WorldModel } from "@/types/worlds";
import { handleErrorKey } from "@/inject/App";
import { readWorld } from "@/api/worlds";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const { t } = useI18n();

const world = ref<WorldModel>();

onMounted(async () => {
  try {
    const slug: string = route.params.slug as string;
    world.value = await readWorld(slug);
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <main class="container">
    <template v-if="world">
      <h1 class="text-center">{{ world.name ?? world.slug }}</h1>
      <ul>
        <li>
          <RouterLink :to="{ name: 'AspectList', params: { slug: world.slug } }">{{ t("aspects.list") }}</RouterLink>
        </li>
        <li>
          <RouterLink :to="{ name: 'CasteList', params: { slug: world.slug } }">{{ t("castes.list") }}</RouterLink>
        </li>
        <li>
          <RouterLink :to="{ name: 'CustomizationList', params: { slug: world.slug } }">{{ t("customizations.list") }}</RouterLink>
        </li>
        <li>
          <RouterLink :to="{ name: 'EducationList', params: { slug: world.slug } }">{{ t("educations.list") }}</RouterLink>
        </li>
        <li>
          <RouterLink :to="{ name: 'ItemList', params: { slug: world.slug } }">{{ t("items.list") }}</RouterLink>
        </li>
        <li>
          <RouterLink :to="{ name: 'LanguageList', params: { slug: world.slug } }">{{ t("languages.list") }}</RouterLink>
        </li>
        <li>
          <RouterLink :to="{ name: 'PartyList', params: { slug: world.slug } }">{{ t("parties.list") }}</RouterLink>
        </li>
        <li>
          <RouterLink :to="{ name: 'PersonalityList', params: { slug: world.slug } }">{{ t("personalities.list") }}</RouterLink>
        </li>
      </ul>
    </template>
  </main>
</template>
