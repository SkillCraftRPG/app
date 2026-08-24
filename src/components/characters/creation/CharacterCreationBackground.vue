<template>
  <form @submit.prevent="handleSubmit(submit)">
    <h2 class="h3">{{ t("characters.background.title") }}</h2>
    <p class="text-body-secondary">{{ t("characters.background.help") }}</p>
    <BackgroundField class="mb-3" v-model="background" />
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
import { onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import BackgroundField from "@/components/characters/profile/BackgroundField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import { useCharacterStore } from "@/stores/character";
import { useForm } from "@/forms";

const character = useCharacterStore();
const { t } = useI18n();

defineEmits<{
  (e: "abandon"): void;
}>();

const background = ref<string>("");

const { handleSubmit } = useForm();
function submit(): void {
  character.saveBackground(background.value);
}

onMounted(() => (background.value = character.creation.background));
</script>
