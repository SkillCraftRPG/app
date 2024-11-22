<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import EducationIcon from "@/components/educations/EducationIcon.vue";
import type { CharacterModel } from "@/types/characters";

const { t } = useI18n();

defineProps<{
  character: CharacterModel;
}>();

// TODO(fpion): button-links text should be centered
</script>

<template>
  <AppInput disabled floating id="education" label="educations.select.label" :model-value="character.education.name" validation="server">
    <template #append>
      <RouterLink :to="{ name: 'EducationEdit', params: { id: character.education.id } }" class="btn btn-primary" target="_blank">
        <EducationIcon /> {{ t("actions.view") }}
      </RouterLink>
    </template>
  </AppInput>
</template>
