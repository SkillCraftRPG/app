<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import CustomizationIcon from "./CustomizationIcon.vue";
import EditIcon from "@/components/shared/EditIcon.vue";
import type { CustomizationModel } from "@/types/customizations";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  customization: CustomizationModel;
  edit?: boolean | string;
}>();

const target = computed<"_blank" | undefined>(() => (parseBoolean(props.edit) ? undefined : "_blank"));
</script>

<template>
  <span>
    <RouterLink :to="{ name: 'CustomizationEdit', params: { id: customization.id } }" :target="target">
      <EditIcon v-if="parseBoolean(edit)" />
      <CustomizationIcon v-else />
    </RouterLink>
    {{ " " }}
    <RouterLink :to="{ name: 'CustomizationEdit', params: { id: customization.id } }" :target="target">{{ customization.name }}</RouterLink>
  </span>
</template>
