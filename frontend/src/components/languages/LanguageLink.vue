<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import EditIcon from "@/components/shared/EditIcon.vue";
import LanguageIcon from "./LanguageIcon.vue";
import type { LanguageModel } from "@/types/languages";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  edit?: boolean | string;
  language: LanguageModel;
}>();

const target = computed<"_blank" | undefined>(() => (parseBoolean(props.edit) ? undefined : "_blank"));
</script>

<template>
  <span>
    <RouterLink :to="{ name: 'LanguageEdit', params: { id: language.id } }" :target="target">
      <EditIcon v-if="parseBoolean(edit)" />
      <LanguageIcon v-else />
    </RouterLink>
    {{ " " }}
    <RouterLink :to="{ name: 'LanguageEdit', params: { id: language.id } }" :target="target">{{ language.name }}</RouterLink>
  </span>
</template>
