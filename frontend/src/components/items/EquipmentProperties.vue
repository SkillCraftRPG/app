<script setup lang="ts">
import DefenseInput from "./DefenseInput.vue";
import ResistanceInput from "./ResistanceInput.vue";
import type { EquipmentPropertiesModel } from "@/types/items";

const props = defineProps<{
  modelValue: EquipmentPropertiesModel;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: EquipmentPropertiesModel): void;
}>();

function setDefense(defense?: number): void {
  const properties: EquipmentPropertiesModel = { ...props.modelValue, defense: defense ?? 0 };
  emit("update:model-value", properties);
}
function setResistance(resistance?: number): void {
  const properties: EquipmentPropertiesModel = { ...props.modelValue, resistance };
  emit("update:model-value", properties);
}
</script>

<template>
  <div>
    <div class="row">
      <DefenseInput class="col-lg-6" :model-value="modelValue.defense" required @update:model-value="setDefense" />
      <ResistanceInput
        class="col-lg-6"
        label="items.equipment.resistance"
        :model-value="modelValue.resistance"
        placeholder="items.equipment.resistance"
        @update:model-value="setResistance"
      />
    </div>
  </div>
</template>
