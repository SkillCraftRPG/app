<template>
  <div>
    <FormInput
      :id="id"
      :label="t(label)"
      :model-value="modelValue"
      :placeholder="t(label)"
      ref="inputRef"
      :required="required"
      :rules="rules"
      type="password"
      @update:model-value="$emit('update:model-value', $event ?? '')"
    />
    <template v-if="enforcePolicy">
      <TarProgress class="mt-2" :label="t(strength.label)" :value="strength.score * 100" :variant="strength.variant" />
      <ul class="list-unstyled mt-2 mb-0 small">
        <li v-for="rule in evaluation" :key="rule.key" :class="rule.success ? 'text-success' : 'text-danger'">
          <font-awesome-icon :icon="rule.success ? 'fas fa-check' : 'fas fa-xmark'" />&nbsp;{{ t(`account.password.rules.${rule.key}`) }}
        </li>
      </ul>
    </template>
  </div>
</template>

<script setup lang="ts">
import type { ValidationRuleSet } from "logitar-validation";
import { computed, ref } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import TarProgress from "@/components/tar/TarProgress.vue";
import type { ProgressVariant } from "@/types/tar/progress";

const MINIMUM_LENGTH: number = 8;
const UNIQUE_CHARACTERS: number = 8;
const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    policy?: boolean | string;
    required?: boolean | string;
  }>(),
  {
    id: "password",
    label: "account.password.label",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

const inputRef = ref<InstanceType<typeof FormInput> | null>();

const enforcePolicy = computed<boolean>(() => parseBoolean(props.policy) ?? false);
const rules = computed<ValidationRuleSet>(() => ({
  minimumLength: enforcePolicy.value ? MINIMUM_LENGTH : false,
  uniqueCharacters: enforcePolicy.value ? UNIQUE_CHARACTERS : false,
  containsLowercase: enforcePolicy.value,
  containsUppercase: enforcePolicy.value,
  containsDigits: enforcePolicy.value,
  containsNonAlphanumeric: enforcePolicy.value,
}));

type Rule = {
  key: string;
  success: boolean;
};
const evaluation = computed<Rule[]>(() => {
  const password: string = props.modelValue ?? "";
  return [
    { key: "min", success: password.length >= MINIMUM_LENGTH },
    { key: "unique", success: new Set(password).size >= UNIQUE_CHARACTERS },
    { key: "lower", success: /\p{Ll}/u.test(password) },
    { key: "upper", success: /\p{Lu}/u.test(password) },
    { key: "digit", success: /\p{Nd}/u.test(password) },
    { key: "special", success: /[^\p{Ll}\p{Lu}\p{Nd}]/u.test(password) },
  ];
});

type Strength = {
  label: string;
  score: number;
  variant: ProgressVariant;
};
const strength = computed<Strength>(() => {
  const score: number = evaluation.value.filter((criteria) => criteria.success).length / evaluation.value.length;
  let key: string = "weak";
  let variant: ProgressVariant = "danger";
  if (score >= 1) {
    key = "strong";
    variant = "success";
  } else if (score >= 0.5) {
    key = "medium";
    variant = "warning";
  }
  const label: string = `account.password.strength.${key}`;
  return { label, score, variant };
});

function focus(): void {
  inputRef.value?.focus();
}
defineExpose({ focus });
</script>
