<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";

import type { CasteModel, FeatureModel } from "@/types/castes";

const { orderBy } = arrayUtils;

const props = defineProps<{
  caste: CasteModel;
}>();

const features = computed<FeatureModel[]>(() => orderBy(props.caste.features, "name"));
</script>

<template>
  <div>
    <template v-for="(feature, index) in features" :key="feature.id">
      {{ feature.name }}
      <br v-if="index < features.length - 1" />
    </template>
  </div>
</template>
