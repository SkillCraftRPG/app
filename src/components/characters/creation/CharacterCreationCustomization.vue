<template>
  <section>
    <h2 class="h3">{{ t("characters.creation.customization.title") }}</h2>
    <section>
      <h3 class="h5">{{ t("characters.creation.customization.identity.lead") }}</h3>
      <p class="text-body-secondary">{{ t("characters.creation.customization.identity.help") }}</p>
      <form @submit.prevent="handleSubmit(submit)">
        <div class="row">
          <div class="col-md-6">
            <NameField class="mb-3" required v-model="name" />
          </div>
          <div class="col-md-6">
            <DominantHandRadio v-model="dominantHand" />
          </div>
        </div>
      </form>
    </section>
    <LoadingSpinner v-if="isLoading" />
    <section v-else-if="customizationList.length">
      <h3 class="h5">{{ t("customizations.title") }}</h3>
      <p class="text-body-secondary">{{ t("characters.creation.customization.options.help") }}</p>
      <p aria-live="polite" :class="customizationStatus.classes">{{ customizationStatus.text }}</p>
      <div class="row">
        <div v-for="customization in customizationList" :key="customization.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
          <CustomizationCard
            class="d-flex flex-column h-100"
            clickable
            :customization="customization"
            :selected="customizations.has(customization.id)"
            selection="multiple"
            @click="toggle(customization)"
          />
        </div>
      </div>
    </section>
    <div class="d-flex justify-content-between">
      <div class="d-flex gap-2">
        <TarButton icon="fas fa-xmark" outline :text="t('actions.abandon')" variant="danger" @click="$emit('abandon')" />
        <TarButton icon="fas fa-arrow-left" outline :text="t('actions.previous')" variant="secondary" @click="character.goBack" />
      </div>
      <TarButton :disabled="!canSubmit" icon="fas fa-arrow-right" :text="t('actions.next')" @click="handleSubmit(submit)" />
    </div>
  </section>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import CustomizationCard from "@/components/customizations/CustomizationCard.vue";
import DominantHandRadio from "@/components/characters/DominantHandRadio.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import NameField from "@/components/shared/NameField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { Customization } from "@/types/customizations";
import type { DominantHand } from "@/types/characters";
import type { SearchResults } from "@/types/search";
import { listCustomizations } from "@/api/customizations";
import { useCharacterStore } from "@/stores/character";
import { useForm } from "@/forms";

const character = useCharacterStore();
const { orderBy } = arrayUtils;
const { t } = useI18n();

const emit = defineEmits<{
  (e: "abandon"): void;
  (e: "error", value: unknown): void;
}>();

const customizationList = ref<Customization[]>([]);
const customizations = ref<Set<string>>(new Set());
const dominantHand = ref<DominantHand | null>(null);
const isLoading = ref<boolean>(true);
const name = ref<string>("");

const disabilities = computed<number>(
  () => customizationList.value.filter((customization) => customizations.value.has(customization.id) && customization.kind === "Disability").length,
);
const gifts = computed<number>(
  () => customizationList.value.filter((customization) => customizations.value.has(customization.id) && customization.kind === "Gift").length,
);

type CustomizationStatus = {
  classes: string[];
  text: string;
};
const customizationStatus = computed<CustomizationStatus>(() => {
  if (!customizations.value.size) {
    return {
      classes: ["text-body-secondary"],
      text: t("characters.creation.customization.options.none"),
    };
  }

  if (gifts.value === disabilities.value) {
    return {
      classes: ["text-success"],
      text: t("characters.creation.customization.options.selected", gifts.value),
    };
  }

  const text: string =
    gifts.value > disabilities.value
      ? t("characters.creation.customization.options.disabilities", gifts.value - disabilities.value)
      : t("characters.creation.customization.options.gifts", disabilities.value - gifts.value);
  return { classes: ["text-warning"], text };
});

const canSubmit = computed<boolean>(() => isValid.value && gifts.value === disabilities.value);

function toggle(customization: Customization): void {
  if (customizations.value.has(customization.id)) {
    customizations.value.delete(customization.id);
  } else {
    customizations.value.add(customization.id);
  }
}

const { handleSubmit, isValid } = useForm();
function submit(): void {
  if (canSubmit.value) {
    character.saveCustomization(
      name.value,
      customizationList.value.filter((customization) => customizations.value.has(customization.id)),
      dominantHand.value,
    );
  }
}

onMounted(async () => {
  try {
    name.value = character.creation.name;
    dominantHand.value = character.creation.dominantHand ?? null;

    const customizationResults: SearchResults<Customization> = await listCustomizations();
    customizationList.value = orderBy(customizationResults.items, "name");

    const customizationIds: Set<string> = new Set(character.creation.customizations.map((customization) => customization.id));
    customizationList.value.forEach((customization) => {
      if (customizationIds.has(customization.id)) {
        customizations.value.add(customization.id);
      }
    });
  } catch (e: unknown) {
    emit("error", e);
  } finally {
    isLoading.value = false;
  }
});
</script>
