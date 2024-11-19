<script setup lang="ts">
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import type { WorldModel } from "@/types/worlds";
import { handleErrorKey } from "@/inject/App";
import { readWorld } from "@/api/worlds";
import type { ApiError } from "@/types/api";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { t } = useI18n();

const world = ref<WorldModel>();

onMounted(async () => {
  try {
    const slug: string = route.params.slug as string;
    world.value = await readWorld(slug);
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (status === 404) {
      router.push({ path: "/not-found" });
    } else {
      handleError(e);
    }
  }
});
</script>

<template>
  <main class="container">
    <template v-if="world">
      <h1 class="text-center">{{ world.name ?? world.slug }}</h1>
      <div class="mb-3 row">
        <AppBreadcrumb class="col" :current="world.name ?? world.slug" @error="handleError" />
        <div class="col">
          <RouterLink class="btn btn-primary float-end" :to="{ name: 'WorldEdit' }">
            <font-awesome-icon icon="fas fa-edit" /> {{ t("actions.edit") }}
          </RouterLink>
        </div>
      </div>
      <ul>
        <li>
          <RouterLink :to="{ name: 'AspectList', params: { slug: world.slug } }">{{ t("aspects.list") }}</RouterLink>
        </li>
        <li>
          <RouterLink :to="{ name: 'PersonalityList', params: { slug: world.slug } }">{{ t("personalities.list") }}</RouterLink>
        </li>
        <li>
          <RouterLink :to="{ name: 'CasteList', params: { slug: world.slug } }">{{ t("castes.list") }}</RouterLink>
        </li>
        <li>
          <RouterLink :to="{ name: 'EducationList', params: { slug: world.slug } }">{{ t("educations.list") }}</RouterLink>
        </li>
        <li>
          <RouterLink :to="{ name: 'LanguageList', params: { slug: world.slug } }">{{ t("languages.list") }}</RouterLink>
        </li>
        <li>
          <RouterLink :to="{ name: 'LineageList', params: { slug: world.slug } }">{{ t("lineages.list") }}</RouterLink>
        </li>
        <li>
          <RouterLink :to="{ name: 'ItemList', params: { slug: world.slug } }">{{ t("items.list") }}</RouterLink>
        </li>
        <li>
          <RouterLink :to="{ name: 'CustomizationList', params: { slug: world.slug } }">{{ t("customizations.list") }}</RouterLink>
        </li>
        <li>
          <RouterLink :to="{ name: 'TalentList', params: { slug: world.slug } }">{{ t("talents.list") }}</RouterLink>
        </li>
        <li>
          <RouterLink :to="{ name: 'PartyList', params: { slug: world.slug } }">{{ t("parties.list") }}</RouterLink>
        </li>
        <li>
          <RouterLink :to="{ name: 'CharacterList', params: { slug: world.slug } }">{{ t("characters.list") }}</RouterLink>
        </li>
      </ul>
    </template>
  </main>
</template>
