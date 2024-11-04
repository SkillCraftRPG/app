<script setup lang="ts">
import CapacityInput from "./CapacityInput.vue";
import VolumeInput from "./VolumeInput.vue";
import type { ContainerPropertiesModel } from "@/types/items";

const props = defineProps<{
  modelValue: ContainerPropertiesModel;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: ContainerPropertiesModel): void;
}>();

function setCapacity(capacity?: number): void {
  const properties: ContainerPropertiesModel = { ...props.modelValue };
  properties.capacity = capacity;
  emit("update:model-value", properties);
}
function setVolume(volume?: number): void {
  const properties: ContainerPropertiesModel = { ...props.modelValue };
  properties.volume = volume;
  emit("update:model-value", properties);
}
</script>

<template>
  <div>
    <div class="row">
      <CapacityInput class="col-lg-6" :model-value="modelValue.capacity" @update:model-value="setCapacity" />
      <VolumeInput class="col-lg-6" :model-value="modelValue.volume" @update:model-value="setVolume" />
    </div>
  </div>
</template>
