<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import PersonalitySelect from "@/components/personalities/PersonalitySelect.vue";
import type { PersonalityModel } from "@/types/personalities";

const { t } = useI18n();

const personality = ref<PersonalityModel>();
</script>

<template>
  <div>
    <h3>{{ t("characters.steps.personality") }}</h3>
    <form>
      <PersonalitySelect :model-value="personality?.id" required @selected="personality = $event" />
      <!-- TODO(fpion): Customizations -->
    </form>
  </div>
</template>
