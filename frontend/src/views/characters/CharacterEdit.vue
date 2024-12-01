<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import CharacterAttributes from "@/components/characters/abilities/CharacterAttributes.vue";
import CharacterBonuses from "@/components/characters/bonuses/CharacterBonuses.vue";
import CharacterCharacteristics from "@/components/characters/characteristics/CharacterCharacteristics.vue";
import CharacterLanguages from "@/components/characters/languages/CharacterLanguages.vue";
import CharacterLevelUps from "@/components/characters/levelUp/CharacterLevelUps.vue";
import CharacterSkills from "@/components/characters/skills/CharacterSkills.vue";
import CharacterSpeeds from "@/components/characters/abilities/CharacterSpeeds.vue";
import CharacterStatistics from "@/components/characters/abilities/CharacterStatistics.vue";
import CharacterTalents from "@/components/characters/talents/CharacterTalents.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { ApiError } from "@/types/api";
import type { CharacterAttributes as CharacterAttributesT, CharacterModel } from "@/types/characters";
import { handleErrorKey } from "@/inject/App";
import { readCharacter } from "@/api/characters";
import { calculateAttributes } from "@/helpers/characterUtils";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { t } = useI18n();

const character = ref<CharacterModel>();

const attributes = computed<CharacterAttributesT | undefined>(() => (character.value ? calculateAttributes(character.value) : undefined));

function onUpdated(value: CharacterModel): void {
  character.value = value;
}

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      character.value = await readCharacter(id);
    }
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
    <template v-if="character">
      <h1>{{ character.name }}</h1>
      <AppBreadcrumb
        :current="character.name"
        :parent="{ route: { name: 'CharacterList' }, text: t('characters.list') }"
        :world="character.world"
        @error="handleError"
      />
      <StatusDetail :aggregate="character" />
      <TarTabs>
        <TarTab id="characteristics" :title="t('characters.characteristics')">
          <CharacterCharacteristics :character="character" @error="handleError" @updated="onUpdated" />
        </TarTab>
        <TarTab v-if="attributes" active id="abilities" :title="t('characters.abilities')">
          <CharacterAttributes :attributes="attributes" :character="character" />
          <CharacterStatistics :attributes="attributes" :character="character" />
          <CharacterSpeeds :character="character" />
        </TarTab>
        <TarTab id="skills" :title="t('game.skills')">
          <CharacterSkills />
        </TarTab>
        <TarTab id="languages" :title="t('languages.list')">
          <CharacterLanguages :character="character" @error="handleError" @updated="onUpdated" />
        </TarTab>
        <TarTab id="talents" :title="t('talents.list')">
          <CharacterTalents :character="character" @error="handleError" @updated="onUpdated" />
        </TarTab>
        <!-- TODO(fpion): Powers -->
        <!-- TODO(fpion): Inventory, Attacks, Dodge & Defense -->
        <TarTab id="bonuses" :title="t('characters.bonuses.label')">
          <CharacterBonuses :character="character" @error="handleError" @updated="onUpdated" />
        </TarTab>
        <TarTab id="levels" :title="t('characters.levels.label')">
          <CharacterLevelUps :character="character" @error="handleError" @updated="onUpdated" />
        </TarTab>
        <!-- TODO(fpion): Notes -->
      </TarTabs>
    </template>
  </main>
</template>
