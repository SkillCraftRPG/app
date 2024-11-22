<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import CasteIcon from "@/components/castes/CasteIcon.vue";
import type { CharacterModel } from "@/types/characters";

const { t } = useI18n();

defineProps<{
  character: CharacterModel;
}>();

// TODO(fpion): button-links text should be centered
</script>

<template>
  <AppInput disabled floating id="caste" label="castes.select.label" :model-value="character.caste.name" validation="server">
    <template #append>
      <RouterLink :to="{ name: 'CasteEdit', params: { id: character.caste.id } }" class="btn btn-primary" target="_blank">
        <CasteIcon /> {{ t("actions.view") }}
      </RouterLink>
    </template>
  </AppInput>
</template>
