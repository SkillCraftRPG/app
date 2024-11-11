<script setup lang="ts">
import { TarBadge, TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import TraitEdit from "./TraitEdit.vue";
import type { TraitPayload, TraitStatus } from "@/types/lineages";

const { t } = useI18n();

const props = defineProps<{
  modelValue: TraitStatus[];
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: TraitStatus[]): void;
}>();

function onAdded(trait: TraitPayload): void {
  const traits: TraitStatus[] = [...props.modelValue];
  traits.push({ trait, isRemoved: false, isUpdated: false });
  emit("update:model-value", traits);
}
function onRemoved(index: number): void {
  const traits: TraitStatus[] = [...props.modelValue];
  const trait: TraitStatus = { ...props.modelValue[index] };
  if (trait.trait.id) {
    trait.isRemoved = !trait.isRemoved;
    traits.splice(index, 1, trait);
  } else {
    traits.splice(index, 1);
  }
  emit("update:model-value", traits);
}
function onUpdated(index: number, trait: TraitPayload): void {
  const traits: TraitStatus[] = [...props.modelValue];
  const updated: TraitStatus = { ...props.modelValue[index], trait, isUpdated: true };
  traits.splice(index, 1, updated);
  emit("update:model-value", traits);
}
</script>

<template>
  <div>
    <h3>{{ t("lineages.traits.label") }}</h3>
    <div class="mb-3">
      <TraitEdit @saved="onAdded" />
    </div>
    <table v-if="modelValue.length > 0" class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("name") }}</th>
          <th scope="col">{{ t("description") }}</th>
          <th scope="col">{{ t("lineages.traits.status.label") }}</th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(trait, index) in modelValue" :key="index">
          <td>{{ trait.trait.name }}</td>
          <td class="description">{{ trait.trait.description }}</td>
          <td>
            <TarBadge v-if="!trait.trait.id" variant="info">{{ t("lineages.traits.status.added") }}</TarBadge>
            <TarBadge v-else-if="trait.isRemoved" variant="danger">{{ t("lineages.traits.status.removed") }}</TarBadge>
            <TarBadge v-else-if="trait.isUpdated" variant="info">{{ t("lineages.traits.status.updated") }}</TarBadge>
            <template v-else>{{ "—" }}</template>
          </td>
          <td>
            <TraitEdit class="me-1" :trait="trait.trait" @saved="onUpdated(index, $event)" />
            <TarButton
              class="ms-1"
              :icon="trait.isRemoved ? 'fas fa-trash-arrow-up' : 'fas fa-trash'"
              :text="t(trait.isRemoved ? 'actions.restore' : 'actions.remove')"
              :variant="trait.isRemoved ? 'warning' : 'danger'"
              @click="onRemoved(index)"
            />
          </td>
        </tr>
      </tbody>
    </table>
    <p v-else>{{ t("lineages.traits.empty") }}</p>
  </div>
</template>

<style scoped>
.description {
  max-width: 800px;
}
</style>
