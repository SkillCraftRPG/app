<script setup lang="ts">
import { TarProgress } from "logitar-vue3-ui";
import { computed, inject, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import Step1Lineage from "@/components/characters/creation/Step1Lineage/Step1Lineage.vue";
import Step2Personality from "@/components/characters/creation/Step2Personality/Step2Personality.vue";
import Step3Aspects from "@/components/characters/creation/Step3Aspects/Step3Aspects.vue";
import Step4Attributes from "@/components/characters/creation/Step4Attributes/Step4Attributes.vue";
import Step5Background from "@/components/characters/creation/Step5Background/Step5Background.vue";
import Step6Talents from "@/components/characters/creation/Step6Talents/Step6Talents.vue";
import type { CharacterModel, CreateCharacterPayload } from "@/types/characters";
import { createCharacter } from "@/api/characters";
import { handleErrorKey } from "@/inject/App";
import { useCharacterStore } from "@/stores/character";
import { useToastStore } from "@/stores/toast";

const character = useCharacterStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const isSubmitting = ref<boolean>(false);

const progress = computed<number>(() => (isSubmitting.value ? 100.0 : ((character.step - 1) * 100.0) / 6.0));

function onAbandon(): void {
  router.push({ name: "CharacterList" });
}

async function onComplete(): Promise<void> {
  const { step1, step2, step3, step4, step5, step6 } = character.creation;
  if (!isSubmitting.value && step1 && step2 && step3 && step4 && step5 && step6) {
    isSubmitting.value = true;
    try {
      const payload: CreateCharacterPayload = {
        name: step1.name,
        player: step1.player,
        lineageId: step1.nation?.id ?? step1.species.id,
        height: step1.height / 100.0,
        weight: step1.weight,
        age: step1.age,
        languageIds: step1.languages.map(({ id }) => id),
        natureId: step2.nature.id,
        customizationIds: step2.customizations.map(({ id }) => id),
        aspectIds: step3.aspects.map(({ id }) => id),
        attributes: step4.attributes,
        casteId: step5.caste.id,
        educationId: step5.education.id,
        talentIds: step6.talents.map(({ id }) => id),
        startingWealth: step5.item && step5.quantity > 0 ? { itemId: step5.item.id, quantity: step5.quantity } : undefined,
      };
      const created: CharacterModel = await createCharacter(payload);
      character.reset();
      toasts.success("characters.created");
      router.push({ name: "CharacterEdit", params: { id: created.id } });
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isSubmitting.value = false;
    }
  }
}
</script>

<template>
  <main class="container">
    <h1>{{ t("characters.create") }}</h1>
    <AppBreadcrumb :current="t('characters.create')" :parent="{ route: { name: 'CharacterList' }, text: t('characters.list') }" @error="handleError" />
    <TarProgress :aria-label="t('characters.steps.progress')" class="mb-3" :value="progress" />
    <Step1Lineage v-if="character.step === 1" @abandon="onAbandon" @error="handleError" />
    <Step2Personality v-if="character.step === 2" @error="handleError" />
    <Step3Aspects v-if="character.step === 3" @error="handleError" />
    <Step4Attributes v-if="character.step === 4" @error="handleError" />
    <Step5Background v-if="character.step === 5" @error="handleError" />
    <Step6Talents v-if="character.step === 6" :loading="isSubmitting" @complete="onComplete" @error="handleError" />
  </main>
</template>
