<script setup lang="ts">
import { TarBadge, TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import FeatureEdit from "./FeatureEdit.vue";
import type { FeaturePayload, FeatureStatus, FeatureUpdated } from "@/types/castes";

const { t } = useI18n();

defineProps<{
  features: FeatureStatus[];
}>();

defineEmits<{
  (e: "added", feature: FeaturePayload): void;
  (e: "removed", index: number): void;
  (e: "updated", event: FeatureUpdated): void;
}>();
</script>

<template>
  <div>
    <h3>{{ t("castes.features.label") }}</h3>
    <div class="mb-3">
      <FeatureEdit @saved="$emit('added', $event)" />
    </div>
    <table v-if="features.length > 0" class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("name") }}</th>
          <th scope="col">{{ t("description") }}</th>
          <th scope="col">{{ t("castes.features.status.label") }}</th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(feature, index) in features" :key="index">
          <td>{{ feature.feature.name }}</td>
          <td class="description">{{ feature.feature.description }}</td>
          <td>
            <TarBadge v-if="!feature.feature.id" variant="info">{{ t("castes.features.status.added") }}</TarBadge>
            <TarBadge v-else-if="feature.isRemoved" variant="danger">{{ t("castes.features.status.removed") }}</TarBadge>
            <TarBadge v-else-if="feature.isUpdated" variant="info">{{ t("castes.features.status.updated") }}</TarBadge>
            <template v-else>{{ "—" }}</template>
          </td>
          <td>
            <FeatureEdit class="me-1" :feature="feature.feature" @saved="$emit('updated', { index, feature: $event })" />
            <TarButton
              class="ms-1"
              :icon="feature.isRemoved ? 'fas fa-trash-arrow-up' : 'fas fa-trash'"
              :text="t(feature.isRemoved ? 'actions.restore' : 'actions.remove')"
              :variant="feature.isRemoved ? 'warning' : 'danger'"
              @click="$emit('removed', index)"
            />
          </td>
        </tr>
      </tbody>
    </table>
    <p v-else>{{ t("castes.features.empty") }}</p>
  </div>
</template>

<style scoped>
.description {
  max-width: 800px;
}
</style>
