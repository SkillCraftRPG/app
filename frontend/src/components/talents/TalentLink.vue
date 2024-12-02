<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import EditIcon from "@/components/shared/EditIcon.vue";
import TalentIcon from "./TalentIcon.vue";
import type { TalentModel } from "@/types/talents";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  edit?: boolean | string;
  talent: TalentModel;
}>();

const target = computed<"_blank" | undefined>(() => (parseBoolean(props.edit) ? undefined : "_blank"));
</script>

<template>
  <span>
    <RouterLink :to="{ name: 'TalentEdit', params: { id: talent.id } }" :target="target">
      <EditIcon v-if="parseBoolean(edit)" />
      <TalentIcon v-else />
    </RouterLink>
    {{ " " }}
    <RouterLink :to="{ name: 'TalentEdit', params: { id: talent.id } }" :target="target">{{ talent.name }}</RouterLink>
  </span>
</template>
