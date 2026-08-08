<template>
  <form @submit.prevent="handleSubmit(submit)">
    <h2 class="h3">{{ t("characters.appearance.title") }}</h2>
    <p class="text-body-secondary">{{ t("characters.appearance.help") }}</p>
    <section>
      <h3 class="h5">{{ t("lineages.physical.size") }}</h3>
      <div class="row">
        <div class="col-md-6">
          <div class="mb-3">
            <div class="fw-bold">{{ t("game.size.category.label") }}</div>
            <div>{{ sizeCategory }}</div>
          </div>
        </div>
        <div class="col-md-6">
          <HeightField class="mb-3" :roll="heightRoll" v-model="height" />
        </div>
      </div>
    </section>
    <div class="d-flex justify-content-between">
      <div class="d-flex gap-2">
        <TarButton icon="fas fa-xmark" outline :text="t('actions.abandon')" variant="danger" @click="$emit('abandon')" />
        <TarButton icon="fas fa-arrow-left" outline :text="t('actions.previous')" variant="secondary" @click="character.goBack" />
      </div>
      <TarButton icon="fas fa-arrow-right" :text="t('actions.next')" type="submit" />
    </div>
  </form>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import HeightField from "@/components/characters/HeightField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { SizeCategory } from "@/types/game";
import { useCharacterStore } from "@/stores/character";
import { useForm } from "@/forms";

const character = useCharacterStore();
const { t } = useI18n();

defineEmits<{
  (e: "abandon"): void;
}>();

const height = ref<number>(0);

const heightRoll = computed<string>(() => character.creation.ethnicity?.size.height ?? character.creation.species?.size.height ?? "");
const sizeCategory = computed<string>(() => {
  const category: SizeCategory = character.creation.ethnicity?.size.category ?? character.creation.species?.size.category ?? "Medium";
  return t(`game.size.category.options.${category}`);
});

const { handleSubmit } = useForm();
function submit(): void {
  console.log("Submitting!"); // TODO(fpion): implement
}

onMounted(() => {
  // TODO(fpion): implement
});
</script>
