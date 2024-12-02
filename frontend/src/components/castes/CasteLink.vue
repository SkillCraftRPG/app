<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import EditIcon from "@/components/shared/EditIcon.vue";
import CasteIcon from "./CasteIcon.vue";
import type { CasteModel } from "@/types/castes";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  caste: CasteModel;
  edit?: boolean | string;
}>();

const target = computed<"_blank" | undefined>(() => (parseBoolean(props.edit) ? undefined : "_blank"));
</script>

<template>
  <span>
    <RouterLink :to="{ name: 'CasteEdit', params: { id: caste.id } }" :target="target">
      <EditIcon v-if="parseBoolean(edit)" />
      <CasteIcon v-else />
    </RouterLink>
    {{ " " }}
    <RouterLink :to="{ name: 'CasteEdit', params: { id: caste.id } }" :target="target">{{ caste.name }}</RouterLink>
  </span>
</template>
