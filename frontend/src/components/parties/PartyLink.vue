<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import EditIcon from "@/components/shared/EditIcon.vue";
import PartyIcon from "./PartyIcon.vue";
import type { PartyModel } from "@/types/parties";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  edit?: boolean | string;
  party: PartyModel;
}>();

const target = computed<"_blank" | undefined>(() => (parseBoolean(props.edit) ? undefined : "_blank"));
</script>

<template>
  <span>
    <RouterLink :to="{ name: 'PartyEdit', params: { id: party.id } }" :target="target">
      <EditIcon v-if="parseBoolean(edit)" />
      <PartyIcon v-else />
    </RouterLink>
    {{ " " }}
    <RouterLink :to="{ name: 'PartyEdit', params: { id: party.id } }" :target="target">{{ party.name }}</RouterLink>
  </span>
</template>
