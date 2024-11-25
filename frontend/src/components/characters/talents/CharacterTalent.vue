<script setup lang="ts">
import AppInput from "@/components/shared/AppInput.vue";
import type { TalentModel } from "@/types/talents";

defineProps<{
  talent: TalentModel;
}>();
</script>

<template>
  <AppInput disabled floating id="talent" label="talents.select.label" :model-value="talent.name" validation="server" />
</template>
