<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import AspectIcon from "./AspectIcon.vue";
import EditIcon from "@/components/shared/EditIcon.vue";
import type { AspectModel } from "@/types/aspects";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  aspect: AspectModel;
  edit?: boolean | string;
}>();

const target = computed<"_blank" | undefined>(() => (parseBoolean(props.edit) ? undefined : "_blank"));
</script>

<template>
  <span>
    <RouterLink :to="{ name: 'AspectEdit', params: { id: aspect.id } }" :target="target">
      <EditIcon v-if="parseBoolean(edit)" />
      <AspectIcon v-else />
    </RouterLink>
    {{ " " }}
    <RouterLink :to="{ name: 'AspectEdit', params: { id: aspect.id } }" :target="target">{{ aspect.name }}</RouterLink>
  </span>
</template>
