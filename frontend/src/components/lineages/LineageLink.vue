<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import EditIcon from "@/components/shared/EditIcon.vue";
import LineageIcon from "./LineageIcon.vue";
import type { LineageModel } from "@/types/lineages";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  edit?: boolean | string;
  lineage: LineageModel;
}>();

const target = computed<"_blank" | undefined>(() => (parseBoolean(props.edit) ? undefined : "_blank"));
</script>

<template>
  <span>
    <RouterLink :to="{ name: 'LineageEdit', params: { id: lineage.id } }" :target="target">
      <EditIcon v-if="parseBoolean(edit)" />
      <LineageIcon v-else />
    </RouterLink>
    {{ " " }}
    <RouterLink :to="{ name: 'LineageEdit', params: { id: lineage.id } }" :target="target">{{ lineage.name }}</RouterLink>
  </span>
</template>
