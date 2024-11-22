<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import NatureIcon from "@/components/natures/NatureIcon.vue";
import type { CharacterModel } from "@/types/characters";

const { t } = useI18n();

defineProps<{
  character: CharacterModel;
}>();

// TODO(fpion): button-links text should be centered
</script>

<template>
  <AppInput disabled floating id="nature" label="natures.select.label" :model-value="character.nature.name" validation="server">
    <template #append>
      <RouterLink :to="{ name: 'NatureEdit', params: { id: character.nature.id } }" class="btn btn-primary" target="_blank">
        <NatureIcon /> {{ t("actions.view") }}
      </RouterLink>
    </template>
  </AppInput>
</template>
