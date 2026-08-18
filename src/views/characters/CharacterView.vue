<template>
  <main class="container page">
    <div v-if="character">
      <h1>{{ title }}</h1>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("characters.created.lead") }}</strong> {{ t("characters.created.help", { name: title }) }}
      </TarAlert>
      <StatusDetail class="mb-3" :subject="character" />
      <TarTabs>
        <TarTab active id="overview" :title="t('characters.overview')">
          <CharacterOverview :character="character" @error="handleError" @updated="update" />
        </TarTab>
        <TarTab id="profile" :title="t('characters.profile')">
          <CharacterProfile :character="character" @error="handleError" @updated="update" />
        </TarTab>
        <TarTab id="modifiers" :title="t('characters.modifiers.title')">
          <CharacterModifiers :character="character" @error="handleError" @updated="update" />
        </TarTab>
      </TarTabs>
    </div>
    <LoadingSpinner v-else />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import CharacterModifiers from "@/components/characters/modifiers/CharacterModifiers.vue";
import CharacterOverview from "@/components/characters/CharacterOverview.vue";
import CharacterProfile from "@/components/characters/CharacterProfile.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarTab from "@/components/tar/TarTab.vue";
import TarTabs from "@/components/tar/TarTabs.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { Character } from "@/types/characters";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readCharacter } from "@/api/characters";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";
import { useToastStore } from "@/stores/toast";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const character = ref<Character>();
const isCreated = ref<boolean>(false);

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("characters.title"), to: { name: "Characters" } }));
const title = computed<string>(() => character.value?.name ?? "");

function update(value: Character): void {
  isCreated.value = false;
  character.value = value;
  toasts.success("saved");
}

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    const found: Character = await readCharacter(id);
    isCreated.value = events.shift() === "created";
    character.value = found;
    document.setTitle(title.value);
  } catch (e: unknown) {
    const failure = e as ApiFailure;
    if (failure.status === StatusCodes.NotFound) {
      router.push("/not-found");
    } else {
      handleError(e);
    }
  }
});

// TODO(fpion): reorganize components `characters` directory

// TODO(fpion): Player
// TODO(fpion): Picture
// TODO(fpion): Critical
// TODO(fpion): Talents
// TODO(fpion): Spells
// TODO(fpion): Inventory
// TODO(fpion): Attacks & Defense
// TODO(fpion): Notes
</script>
