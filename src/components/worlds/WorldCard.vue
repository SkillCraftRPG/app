<template>
  <TarCard :title="title"></TarCard>
</template>

<script setup lang="ts">
import { computed } from "vue";

import TarCard from "@/components/tar/TarCard.vue";
import type { World } from "@/types/worlds";

const props = defineProps<{
  world: World;
}>();

const title = computed<string>(() => props.world.name ?? props.world.key);
</script>
