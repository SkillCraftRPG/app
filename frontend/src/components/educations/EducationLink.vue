<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import EditIcon from "@/components/shared/EditIcon.vue";
import EducationIcon from "./EducationIcon.vue";
import type { EducationModel } from "@/types/educations";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  edit?: boolean | string;
  education: EducationModel;
}>();

const target = computed<"_blank" | undefined>(() => (parseBoolean(props.edit) ? undefined : "_blank"));
</script>

<template>
  <span>
    <RouterLink :to="{ name: 'EducationEdit', params: { id: education.id } }" :target="target">
      <EditIcon v-if="parseBoolean(edit)" />
      <EducationIcon v-else />
    </RouterLink>
    {{ " " }}
    <RouterLink :to="{ name: 'EducationEdit', params: { id: education.id } }" :target="target">{{ education.name }}</RouterLink>
  </span>
</template>
