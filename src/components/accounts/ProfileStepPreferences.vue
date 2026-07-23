<template>
  <div>
    <div class="row">
      <div class="col-md-6">
        <DateOfBirthInput class="mb-3" :model-value="modelValue.dateOfBirth ?? undefined" @update:model-value="updateDateOfBirth" />
      </div>
      <div class="col-md-6">
        <GenderRadio class="mb-3" :model-value="modelValue.gender" @update:model-value="updateGender" />
      </div>
    </div>
    <div class="row">
      <div class="col-md-6">
        <LocaleRadio class="mb-3" :model-value="modelValue.locale" required @update:model-value="updateLocale" />
      </div>
      <div class="col-md-6">
        <TimeZoneSelect class="mb-3" :model-value="modelValue.timeZone" required @update:model-value="updateTimeZone" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import DateOfBirthInput from "./DateOfBirthInput.vue";
import GenderRadio from "./GenderRadio.vue";
import LocaleRadio from "./LocaleRadio.vue";
import TimeZoneSelect from "./TimeZoneSelect.vue";
import type { Gender, PreferencesInformation } from "@/types/account";

const props = defineProps<{
  modelValue: PreferencesInformation;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: PreferencesInformation): void;
}>();

function updateDateOfBirth(dateOfBirth: Date | null): void {
  emit("update:model-value", { ...props.modelValue, dateOfBirth });
}
function updateGender(gender: Gender | null): void {
  emit("update:model-value", { ...props.modelValue, gender });
}
function updateLocale(locale: string): void {
  emit("update:model-value", { ...props.modelValue, locale });
}
function updateTimeZone(timeZone: string): void {
  emit("update:model-value", { ...props.modelValue, timeZone });
}
</script>
