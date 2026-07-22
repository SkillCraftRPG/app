<template>
  <div class="row">
    <div class="col-md-6">
      <FirstNameInput class="mb-3" :model-value="modelValue.firstName" required @update:model-value="updateFirstName($event)" />
    </div>
    <div class="col-md-6">
      <LastNameInput class="mb-3" :model-value="modelValue.lastName" required @update:model-value="updateLastName($event)" />
    </div>
  </div>
</template>

<script setup lang="ts">
import FirstNameInput from "./FirstNameInput.vue";
import LastNameInput from "./LastNameInput.vue";
import type { PersonalInformation } from "@/types/account";

const props = defineProps<{
  modelValue: PersonalInformation;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: PersonalInformation): void;
}>();

function updateFirstName(firstName: string): void {
  emit("update:model-value", { ...props.modelValue, firstName });
}
function updateLastName(lastName: string): void {
  emit("update:model-value", { ...props.modelValue, lastName });
}
</script>
