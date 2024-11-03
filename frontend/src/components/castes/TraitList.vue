<script setup lang="ts">
import { TarBadge, TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import TraitEdit from "./TraitEdit.vue";
import type { TraitPayload, TraitStatus, TraitUpdated } from "@/types/castes";

const { t } = useI18n();

defineProps<{
  traits: TraitStatus[];
}>();

defineEmits<{
  (e: "added", trait: TraitPayload): void;
  (e: "removed", index: number): void;
  (e: "updated", event: TraitUpdated): void;
}>();
</script>

<template>
  <div>
    <h3>{{ t("castes.traits.label") }}</h3>
    <div class="mb-3">
      <TraitEdit @saved="$emit('added', $event)" />
    </div>
    <table v-if="traits.length > 0" class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("name") }}</th>
          <th scope="col">{{ t("description") }}</th>
          <th scope="col">{{ t("castes.traits.status.label") }}</th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(trait, index) in traits" :key="index">
          <td>{{ trait.trait.name }}</td>
          <td class="description">{{ trait.trait.description }}</td>
          <td>
            <TarBadge v-if="!trait.trait.id" variant="info">{{ t("castes.traits.status.added") }}</TarBadge>
            <TarBadge v-else-if="trait.isRemoved" variant="danger">{{ t("castes.traits.status.removed") }}</TarBadge>
            <TarBadge v-else-if="trait.isUpdated" variant="info">{{ t("castes.traits.status.updated") }}</TarBadge>
            <template v-else>{{ "—" }}</template>
          </td>
          <td>
            <TraitEdit class="me-1" :trait="trait.trait" @saved="$emit('updated', { index, trait: $event })" />
            <TarButton
              class="ms-1"
              :icon="trait.isRemoved ? 'fas fa-trash-arrow-up' : 'fas fa-trash'"
              :text="t(trait.isRemoved ? 'actions.restore' : 'actions.remove')"
              :variant="trait.isRemoved ? 'warning' : 'danger'"
              @click="$emit('removed', index)"
            />
          </td>
        </tr>
      </tbody>
    </table>
    <p v-else>{{ t("castes.traits.empty") }}</p>
  </div>
</template>

<style scoped>
.description {
  max-width: 800px;
}
</style>
