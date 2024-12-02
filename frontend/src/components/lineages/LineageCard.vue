<script setup lang="ts">
import { TarCard } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import LineageIcon from "./LineageIcon.vue";
import type { LineageModel } from "@/types/lineages";

const { t } = useI18n();

defineProps<{
  lineage: LineageModel;
}>();
</script>

<template>
  <TarCard :title="lineage.name">
    <div class="float-end">
      <RouterLink class="btn btn-primary" :to="{ name: 'LineageEdit', params: { id: lineage.id } }"><LineageIcon /> {{ t("actions.edit") }}</RouterLink>
    </div>
  </TarCard>
</template>
