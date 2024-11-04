<script setup lang="ts">
import AttackInput from "./AttackInput.vue";
import ResistanceInput from "./ResistanceInput.vue";
import type { WeaponPropertiesModel } from "@/types/items";

const props = defineProps<{
  modelValue: WeaponPropertiesModel;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: WeaponPropertiesModel): void;
}>();

function setAttack(attack?: number): void {
  const properties: WeaponPropertiesModel = { ...props.modelValue, attack: attack ?? 0 };
  emit("update:model-value", properties);
}
function setResistance(resistance?: number): void {
  const properties: WeaponPropertiesModel = { ...props.modelValue, resistance };
  emit("update:model-value", properties);
}
</script>

<template>
  <div>
    <div class="row">
      <AttackInput class="col-lg-6" :model-value="modelValue.attack" required @update:model-value="setAttack" />
      <ResistanceInput
        class="col-lg-6"
        label="items.weapon.resistance"
        :model-value="modelValue.resistance"
        placeholder="items.weapon.resistance"
        @update:model-value="setResistance"
      />
    </div>
  </div>
</template>
