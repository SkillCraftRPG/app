<script setup lang="ts">
import type { WeaponDamageModel } from "@/types/items";
import { TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import DamageEdit from "./DamageEdit.vue";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue: WeaponDamageModel[];
  }>(),
  {
    id: "damage",
    label: "items.weapon.damage.label",
  },
);

const emit = defineEmits<{
  (e: "update:model-value", value: WeaponDamageModel[]): void;
}>();

function onAdd(): void {
  const list: WeaponDamageModel[] = [...props.modelValue];
  list.push({ roll: "", type: "Piercing" });
  emit("update:model-value", list);
}
function onRemove(index: number): void {
  const list: WeaponDamageModel[] = [...props.modelValue];
  list.splice(index, 1);
  emit("update:model-value", list);
}
function onUpdate(index: number, item: WeaponDamageModel): void {
  const list: WeaponDamageModel[] = [...props.modelValue];
  list.splice(index, 1, item);
  emit("update:model-value", list);
}
</script>

<template>
  <div :id="id">
    <h5>{{ t(label) }}</h5>
    <div class="mb-3">
      <TarButton icon="fas fa-plus" :id="`${id}-add`" :text="t('actions.add')" variant="success" @click="onAdd" />
    </div>
    <DamageEdit
      v-for="(damage, index) in modelValue"
      :key="index"
      class="row"
      :id="`${id}-${index}`"
      :model-value="damage"
      @removed="onRemove(index)"
      @update:model-value="onUpdate(index, $event)"
    />
    <p v-if="modelValue.length === 0">{{ t("items.weapon.damage.empty") }}</p>
  </div>
</template>
