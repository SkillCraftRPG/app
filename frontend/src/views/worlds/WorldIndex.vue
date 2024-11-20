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

/*
 * (❌) Account
 * (❌) Worlds
 * (🚧) Membership
 * (⭐) Parties
 * (⭐) Characters
 * (✅) Lineages
 * (✅) Castes
 * (✅) Educations
 * (✅) Natures
 * (✅) Specializations
 * (✅) Aspects
 * (✅) Customizations
 * (✅) Languages
 * (✅) Items
 * (✅) Talents
 * (✅) Powers
 * (❌) Comments
 * (❌) Documents
 * (❌) Permissions
 * (❌) Player Interface
 */
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
      <div class="row">
        <div class="col-lg-3 col-md-4 col-sm-6 mb-3">
          <RouterLink class="tile" :to="{ name: 'LanguageList', params: { slug: world.slug } }">{{ t("languages.list") }}</RouterLink>
        </div>
        <div class="col-lg-3 col-md-4 col-sm-6 mb-3">
          <RouterLink class="tile" :to="{ name: 'LineageList', params: { slug: world.slug } }">{{ t("lineages.list") }}</RouterLink>
        </div>
        <div class="col-lg-3 col-md-4 col-sm-6 mb-3">
          <RouterLink class="tile" :to="{ name: 'CustomizationList', params: { slug: world.slug } }">{{ t("customizations.list") }}</RouterLink>
        </div>
        <div class="col-lg-3 col-md-4 col-sm-6 mb-3">
          <RouterLink class="tile" :to="{ name: 'NatureList', params: { slug: world.slug } }">{{ t("natures.list") }}</RouterLink>
        </div>
        <div class="col-lg-3 col-md-4 col-sm-6 mb-3">
          <RouterLink class="tile" :to="{ name: 'AspectList', params: { slug: world.slug } }">{{ t("aspects.list") }}</RouterLink>
        </div>
        <div class="col-lg-3 col-md-4 col-sm-6 mb-3">
          <RouterLink class="tile" :to="{ name: 'CasteList', params: { slug: world.slug } }">{{ t("castes.list") }}</RouterLink>
        </div>
        <div class="col-lg-3 col-md-4 col-sm-6 mb-3">
          <RouterLink class="tile" :to="{ name: 'EducationList', params: { slug: world.slug } }">{{ t("educations.list") }}</RouterLink>
        </div>
        <div class="col-lg-3 col-md-4 col-sm-6 mb-3">
          <RouterLink class="tile" :to="{ name: 'TalentList', params: { slug: world.slug } }">{{ t("talents.list") }}</RouterLink>
        </div>
        <div class="col-lg-3 col-md-4 col-sm-6 mb-3">
          <RouterLink class="tile" :to="{ name: 'ItemList', params: { slug: world.slug } }">{{ t("items.list") }}</RouterLink>
        </div>
      </div>
      <div class="row">
        <div class="col-sm-6 mb-3">
          <RouterLink class="tile" :to="{ name: 'PartyList', params: { slug: world.slug } }">{{ t("parties.list") }}</RouterLink>
        </div>
        <div class="col-sm-6 mb-3">
          <RouterLink class="tile" :to="{ name: 'CharacterList', params: { slug: world.slug } }">{{ t("characters.list") }}</RouterLink>
        </div>
      </div>
    </template>
  </main>
</template>

<style scoped>
.tile {
  background-color: var(--bs-tertiary-bg);
  border: 0.25rem solid var(--bs-border-color);
  height: 200px;
  width: 200px;
}
.tile:hover {
  background-color: var(--bs-secondary-bg);
  cursor: pointer;
}
</style>
