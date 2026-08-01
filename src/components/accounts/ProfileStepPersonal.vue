<template>
  <div>
    <EmailDisplay v-if="email" class="mb-3" :email="email" />
    <div class="row">
      <div class="col-md-6">
        <FirstNameField class="mb-3" :model-value="modelValue.firstName" required @update:model-value="updateFirstName($event)" />
      </div>
      <div class="col-md-6">
        <LastNameField class="mb-3" :model-value="modelValue.lastName" required @update:model-value="updateLastName($event)" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import EmailDisplay from "./EmailDisplay.vue";
import FirstNameField from "./FirstNameField.vue";
import LastNameField from "./LastNameField.vue";
import type { Email, PersonalInformation } from "@/types/account";

const props = defineProps<{
  email?: Email;
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
