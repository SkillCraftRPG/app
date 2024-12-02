<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import EditIcon from "@/components/shared/EditIcon.vue";
import CharacterIcon from "./CharacterIcon.vue";
import type { CharacterModel } from "@/types/characters";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  character: CharacterModel;
  edit?: boolean | string;
}>();

const target = computed<"_blank" | undefined>(() => (parseBoolean(props.edit) ? undefined : "_blank"));
</script>

<template>
  <span>
    <RouterLink :to="{ name: 'CharacterEdit', params: { id: character.id } }" :target="target">
      <EditIcon v-if="parseBoolean(edit)" />
      <CharacterIcon v-else />
    </RouterLink>
    {{ " " }}
    <RouterLink :to="{ name: 'CharacterEdit', params: { id: character.id } }" :target="target">{{ character.name }}</RouterLink>
  </span>
</template>
