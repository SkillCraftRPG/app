<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";

import DamageRollInput from "@/components/game/DamageRollInput.vue";
import DamageTypeSelect from "@/components/game/DamageTypeSelect.vue";
import type { WeaponDamageModel } from "@/types/items";
import type { DamageType } from "@/types/game";

const props = withDefaults(
  defineProps<{
    id?: string;
    modelValue: WeaponDamageModel;
  }>(),
  {
    id: "damage",
  },
);

const emit = defineEmits<{
  (e: "removed"): void;
  (e: "update:model-value", value: WeaponDamageModel): void;
}>();

function setRoll(roll?: string): void {
  const model: WeaponDamageModel = { ...props.modelValue, roll: roll ?? "" };
  emit("update:model-value", model);
}
function setType(type?: DamageType): void {
  const model: WeaponDamageModel = { ...props.modelValue, type: type ?? (undefined as any) };
  emit("update:model-value", model);
}
</script>

<template>
  <div>
    <DamageRollInput class="col" :id="`${id}-roll`" :model-value="modelValue.roll" required @update:model-value="setRoll">
      <template #prepend>
        <TarButton icon="fas fa-times" variant="danger" @click="$emit('removed')" />
      </template>
    </DamageRollInput>
    <DamageTypeSelect class="col" :id="`${id}-type`" :model-value="modelValue.type" required @update:model-value="setType" />
  </div>
</template>
