<template>
  <div>
    <div v-for="option in options" :key="option.value" class="form-check mb-3">
      <input
        :checked="modelValue.mode === option.value"
        class="form-check-input"
        :id="`security-${option.value}`"
        name="account-security"
        required
        type="radio"
        :value="option.value"
        @change="updateMode(option.value)"
      />
      <label class="form-check-label" :for="`security-${option.value}`">
        <div class="fw-semibold">{{ t(option.label) }}</div>
        <div class="text-body-secondary">{{ t(option.help) }}</div>
        <div v-if="option.note" class="text-body-secondary mt-1">{{ t(option.note) }}</div>
      </label>
    </div>
    <PasswordInput v-if="isPasswordRequired" class="mb-3" :model-value="modelValue.password" policy required @update:model-value="updatePassword" />
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import PasswordInput from "./PasswordInput.vue";
import type { SecurityInformation, SecurityMode } from "@/types/account";

const { t } = useI18n();

const props = defineProps<{
  modelValue: SecurityInformation;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: SecurityInformation): void;
}>();

type Option = {
  value: SecurityMode;
  label: string;
  help: string;
  note?: string;
};
const options: Option[] = [
  {
    value: "PasswordLess",
    label: "account.profile.completion.security.options.passwordLess.label",
    help: "account.profile.completion.security.options.passwordLess.help",
    note: "account.profile.completion.security.options.passwordLess.note",
  },
  {
    value: "Password",
    label: "account.profile.completion.security.options.password.label",
    help: "account.profile.completion.security.options.password.help",
  },
  {
    value: "MultiFactor",
    label: "account.profile.completion.security.options.multiFactor.label",
    help: "account.profile.completion.security.options.multiFactor.help",
    note: "account.profile.completion.security.options.multiFactor.note",
  },
];

const isPasswordRequired = computed<boolean>(() => props.modelValue.mode !== "PasswordLess");

function updateMode(mode: SecurityMode): void {
  const value: SecurityInformation = { ...props.modelValue, mode };
  if (mode === "PasswordLess") {
    value.password = "";
  }
  emit("update:model-value", value);
}
function updatePassword(password: string): void {
  emit("update:model-value", { ...props.modelValue, password });
}
</script>
