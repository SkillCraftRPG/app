<template>
  <main class="container page">
    <h1>{{ title }}</h1>
    <WorldBreadcrumb :current="t('characters.creation.label')" :parent="breadcrumb" />
    <TarProgress class="mb-3" :value="progress" />
    <!-- TODO(fpion): progress bar -->
    <CharacterCreationAscendancy v-if="step === Step.Ascendancy" @error="handleError" />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, ref, watchEffect } from "vue";
import { useI18n } from "vue-i18n";

import CharacterCreationAscendancy from "@/components/characters/creation/CharacterCreationAscendancy.vue";
import TarProgress from "@/components/tar/TarProgress.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import { handleErrorKey } from "@/inject";
import { useDocument } from "@/composables/document";

const document = useDocument();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const { t } = useI18n();

enum Step {
  Ascendancy = 0,
}

const step = ref<Step>(Step.Ascendancy);

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("characters.title"), to: { name: "Characters" } }));
const progress = computed<number>(() => Math.floor(step.value * 100) / 9);
const title = computed<string>(() => t("characters.creation.title"));

watchEffect(() => document.setTitle(title.value));
</script>
