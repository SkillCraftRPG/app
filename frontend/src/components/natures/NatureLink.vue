<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import EditIcon from "@/components/shared/EditIcon.vue";
import NatureIcon from "./NatureIcon.vue";
import type { NatureModel } from "@/types/natures";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  edit?: boolean | string;
  nature: NatureModel;
}>();

const target = computed<"_blank" | undefined>(() => (parseBoolean(props.edit) ? undefined : "_blank"));
</script>

<template>
  <span>
    <RouterLink :to="{ name: 'NatureEdit', params: { id: nature.id } }" :target="target">
      <EditIcon v-if="parseBoolean(edit)" />
      <NatureIcon v-else />
    </RouterLink>
    {{ " " }}
    <RouterLink :to="{ name: 'NatureEdit', params: { id: nature.id } }" :target="target">{{ nature.name }}</RouterLink>
  </span>
</template>
