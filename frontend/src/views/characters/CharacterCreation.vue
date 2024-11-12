<script setup lang="ts">
import { TarProgress } from "logitar-vue3-ui";
import { computed, inject, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import Step1Lineage from "@/components/characters/creation/Step1Lineage.vue";
import Step2Personality from "@/components/characters/creation/Step2Personality.vue";
import Step3Aspect from "@/components/characters/creation/Step3Aspect.vue";
import type { Step1, Step2, Step3 } from "@/types/characters";
import { handleErrorKey } from "@/inject/App";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const router = useRouter();
const { t } = useI18n();

const isSubmitting = ref<boolean>(false); // TODO(fpion): implement
const step = ref<number>(1);

const progress = computed<number>(() => (isSubmitting.value ? 100.0 : ((step.value - 1) * 100.0) / 6.0));

function onStep1(value?: Step1): void {
  if (value) {
    step.value++; // TODO(fpion): implement
  } else {
    router.push({ name: "CharacterList" });
  }
}
function onStep2(value?: Step2): void {
  if (value) {
    step.value++; // TODO(fpion): implement
  } else {
    step.value--;
  }
}
function onStep3(value?: Step3): void {
  if (value) {
    step.value++; // TODO(fpion): implement
  } else {
    step.value--;
  }
}

/*
 * ( ) AspectsNotFoundException
 * ( ) CasteHasNoSkillTalentException
 * ( ) CasteNotFoundException
 * ( ) CustomizationsCannotIncludePersonalityGiftException
 * ( ) CustomizationsNotFoundException
 * ( ) EducationHasNoSkillTalentException
 * ( ) EducationNotFoundException
 * ( ) InvalidAspectAttributeSelectionException
 * ( ) InvalidCasteEducationSelectionException
 * ( ) InvalidCharacterCustomizationsException
 * (✅) InvalidCharacterLineageException
 * ( ) InvalidExtraAttributesException
 * ( ) InvalidExtraLanguagesException
 * ( ) InvalidSkillTalentSelectionException
 * ( ) InvalidStartingWealthSelectionException
 * ( ) ItemNotFoundException
 * ( ) LanguagesCannotIncludeLineageLanguageException
 * ( ) LanguagesNotFoundException
 * (✅) LineageNotFoundException
 * (✅) NotEnoughAvailableStorageException
 * (✅) PermissionDeniedException
 * (✅) PersonalityNotFoundException
 * ( ) TalentsNotFoundException
 * ( ) ValidationException
 */
</script>

<template>
  <main class="container">
    <h1>{{ t("characters.create") }}</h1>
    <AppBreadcrumb :current="t('characters.create')" :parent="{ route: { name: 'CharacterList' }, text: t('characters.list') }" @error="handleError" />
    <TarProgress :aria-label="t('characters.steps.progress')" class="mb-3" :value="progress" />
    <Step1Lineage v-if="step === 1" @abandon="onStep1()" @continue="onStep1" @error="handleError" />
    <Step2Personality v-if="step === 2" @back="onStep2()" @continue="onStep2" @error="handleError" />
    <Step3Aspect v-if="step === 3" @back="onStep3()" @continue="onStep3" @error="handleError" />
  </main>
</template>
