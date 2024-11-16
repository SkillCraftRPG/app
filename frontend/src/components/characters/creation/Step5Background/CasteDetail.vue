<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";

import MarkdownText from "@/components/shared/MarkdownText.vue";
import type { TraitModel, CasteModel } from "@/types/castes";

const { orderBy } = arrayUtils;

const props = defineProps<{
  caste: CasteModel;
}>();

const features = computed<TraitModel[]>(() => orderBy(props.caste.traits, "name"));
</script>

<template>
  <ul>
    <li v-for="feature in features" :key="feature.id">
      <strong>{{ feature.name }}.</strong> <MarkdownText v-if="feature.description" inline :text="feature.description" />
    </li>
  </ul>
</template>
