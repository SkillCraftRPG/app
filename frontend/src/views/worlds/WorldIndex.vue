<script setup lang="ts">
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import AspectIcon from "@/components/aspects/AspectIcon.vue";
import CasteIcon from "@/components/castes/CasteIcon.vue";
import CharacterIcon from "@/components/characters/CharacterIcon.vue";
import CustomizationIcon from "@/components/customizations/CustomizationIcon.vue";
import EducationIcon from "@/components/educations/EducationIcon.vue";
import ItemIcon from "@/components/items/ItemIcon.vue";
import LanguageIcon from "@/components/languages/LanguageIcon.vue";
import LineageIcon from "@/components/lineages/LineageIcon.vue";
import NatureIcon from "@/components/natures/NatureIcon.vue";
import PartyIcon from "@/components/parties/PartyIcon.vue";
import SpellIcon from "@/components/spells/SpellIcon.vue";
import TalentIcon from "@/components/talents/TalentIcon.vue";
import type { ApiError } from "@/types/api";
import type { WorldModel } from "@/types/worlds";
import { handleErrorKey } from "@/inject/App";
import { readWorld } from "@/api/worlds";

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
      <div class="d-flex flex-column justify-content-center align-items-center">
        <div class="sc-grid-main">
          <RouterLink class="tile" :to="{ name: 'LanguageList', params: { slug: world.slug } }">
            <LanguageIcon class="icon" /> {{ t("languages.list") }}
          </RouterLink>
          <RouterLink class="tile" :to="{ name: 'LineageList', params: { slug: world.slug } }">
            <LineageIcon class="icon" /> {{ t("lineages.list") }}
          </RouterLink>
          <RouterLink class="tile" :to="{ name: 'CustomizationList', params: { slug: world.slug } }">
            <CustomizationIcon class="icon" /> {{ t("customizations.list") }}
          </RouterLink>
          <RouterLink class="tile" :to="{ name: 'NatureList', params: { slug: world.slug } }"><NatureIcon class="icon" /> {{ t("natures.list") }}</RouterLink>
          <RouterLink class="tile" :to="{ name: 'AspectList', params: { slug: world.slug } }"><AspectIcon class="icon" /> {{ t("aspects.list") }}</RouterLink>
          <RouterLink class="tile" :to="{ name: 'CasteList', params: { slug: world.slug } }"><CasteIcon class="icon" /> {{ t("castes.list") }}</RouterLink>
          <RouterLink class="tile" :to="{ name: 'EducationList', params: { slug: world.slug } }">
            <EducationIcon class="icon" /> {{ t("educations.list") }}
          </RouterLink>
          <RouterLink class="tile" :to="{ name: 'TalentList', params: { slug: world.slug } }"><TalentIcon class="icon" /> {{ t("talents.list") }}</RouterLink>
          <div class="tile"><SpellIcon class="icon" /> Pouvoirs</div>
          <RouterLink class="tile" :to="{ name: 'ItemList', params: { slug: world.slug } }"><ItemIcon class="icon" /> {{ t("items.list") }}</RouterLink>
          <div class="tile"><font-awesome-icon class="icon" icon="fas fa-question" /> TODO</div>
          <div class="tile"><font-awesome-icon class="icon" icon="fas fa-question" /> TODO</div>
        </div>
        <div class="sc-grid-secondary">
          <RouterLink class="tile" :to="{ name: 'PartyList', params: { slug: world.slug } }"><PartyIcon class="icon" /> {{ t("parties.list") }}</RouterLink>
          <RouterLink class="tile" :to="{ name: 'CharacterList', params: { slug: world.slug } }">
            <CharacterIcon class="icon" /> {{ t("characters.list") }}
          </RouterLink>
        </div>
      </div>
    </template>
  </main>
</template>

<style scoped>
.sc-grid-main,
.sc-grid-secondary {
  display: grid;
  grid-template-columns: repeat(var(--columns), var(--column-width));
  gap: var(--gap);
  max-width: calc(var(--columns) * var(--column-width) + (var(--columns) - 1) * var(--gap));
  margin-bottom: var(--gap);
}

.sc-grid-main {
  --columns: 1;
  --gap: 1.5rem;
  --column-width: 13.5rem;
  --column-height: 13.5rem;
}

.sc-grid-secondary {
  --columns: 1;
  --gap: 1.5rem;
  --column-width: 13.5rem;
  --column-height: 13.5rem;
}

.tile {
  box-shadow: rgba(99, 99, 99, 0.2) 0px 2px 8px 0px;
  background-color: var(--bs-tertiary-bg);
  border: 1px solid var(--bs-border-color);
  border-radius: 0.75rem;
  width: 100%;
  max-width: var(--column-width);
  height: var(--column-height);
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  font-size: 1.5rem;
  text-decoration: none;
  gap: 0.5rem;
}

.tile:hover {
  background-color: var(--bs-secondary-bg);
  cursor: pointer;
  text-decoration: underline;
}

.sc-grid-secondary .tile {
  aspect-ratio: var(--columns) / 1;
}

.tile .icon {
  font-size: 4.5rem;
}

@media (min-width: 576px) {
  .sc-grid-main {
    --columns: 2;
  }

  .sc-grid-secondary {
    --column-width: 28.5rem;
  }
}

@media (min-width: 768px) {
  .sc-grid-main {
    --columns: 3;
  }

  .sc-grid-secondary {
    --columns: 2;
    --column-width: 21rem;
  }
}

@media (min-width: 992px) {
  .sc-grid-main {
    --columns: 4;
  }

  .sc-grid-secondary {
    --columns: 2;
    --column-width: 28.5rem;
  }
}
</style>
