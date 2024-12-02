<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import EditIcon from "@/components/shared/EditIcon.vue";
import ItemIcon from "./ItemIcon.vue";
import type { ItemModel } from "@/types/items";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  edit?: boolean | string;
  item: ItemModel;
}>();

const target = computed<"_blank" | undefined>(() => (parseBoolean(props.edit) ? undefined : "_blank"));
</script>

<template>
  <span>
    <RouterLink :to="{ name: 'ItemEdit', params: { id: item.id } }" :target="target">
      <EditIcon v-if="parseBoolean(edit)" />
      <ItemIcon v-else />
    </RouterLink>
    {{ " " }}
    <RouterLink :to="{ name: 'ItemEdit', params: { id: item.id } }" :target="target">{{ item.name }}</RouterLink>
  </span>
</template>
