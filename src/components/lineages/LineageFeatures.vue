<template>
  <div>
    <EditLineageFeature class="mb-3" :lineage="lineage" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
    <div v-if="features.length" class="row">
      <div v-for="feature in features" :key="feature.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
        <LineageFeatureCard class="feature-card h-100" :feature="feature">
          <div class="d-flex justify-content-end align-items-center mt-auto pt-2 gap-2">
            <EditLineageFeature :feature="feature" :lineage="lineage" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
            <RemoveLineageFeature :feature="feature" :lineage="lineage" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
          </div>
        </LineageFeatureCard>
      </div>
    </div>
    <p v-else>{{ t("lineages.features.empty") }}</p>
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import EditLineageFeature from "./EditLineageFeature.vue";
import LineageFeatureCard from "./LineageFeatureCard.vue";
import RemoveLineageFeature from "./RemoveLineageFeature.vue";
import type { Lineage, LineageFeature } from "@/types/lineages";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  lineage: Lineage;
}>();

defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Lineage): void;
}>();

const features = computed<LineageFeature[]>(() => orderBy(props.lineage.features, "name"));
</script>

<style scoped>
.feature-card :deep(.card-body) {
  display: flex;
  flex-direction: column;
  height: 100%;
}
</style>
