<template>
  <TarTextarea
    :cols="cols"
    :described-by="textareaDescribedBy"
    :disabled="disabled"
    :floating="floating"
    :id="id"
    :label="label"
    :model-value="modelValue"
    :name="name"
    :placeholder="placeholder ?? label"
    :plaintext="plaintext"
    :readonly="readonly"
    ref="textareaRef"
    :required="textareaRequired"
    :rows="rows"
    :size="size"
    :status="textareaStatus"
    @blur="handleChange"
    @change="handleChange"
    @input="handleChange($event, textareaStatus === 'invalid')"
  >
    <template #before>
      <slot name="before"></slot>
    </template>
    <template #prepend>
      <slot name="prepend"></slot>
    </template>
    <template #label-override>
      <slot name="label-override"></slot>
    </template>
    <template #label-required>
      <slot name="label-required"></slot>
    </template>
    <template #append>
      <slot name="append"></slot>
    </template>
    <template #after>
      <div v-if="error" class="invalid-feedback" :id="feedbackId">
        {{ t(`errors.${error.key}`, error.placeholders) }}
      </div>
      <slot name="after"></slot>
    </template>
  </TarTextarea>
</template>

<script setup lang="ts">
import type { RuleExecutionResult, ValidationResult, ValidationRuleSet } from "logitar-validation";
import { computed, onBeforeUnmount, ref, watch } from "vue";
import { nanoid } from "nanoid";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import TarTextarea from "@/components/tar/TarTextarea.vue";
import type { TextareaOptions, TextareaStatus } from "@/types/tar/textarea";
import { useField } from "@/forms";

const { parseBoolean, parseNumber } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<
    TextareaOptions & {
      rules?: ValidationRuleSet;
    }
  >(),
  {
    floating: true,
    id: () => nanoid(),
  },
);

const textareaRef = ref<InstanceType<typeof TarTextarea> | null>(null);

const error = computed<RuleExecutionResult | undefined>(() => errors.value[0]);
const feedbackId = computed<string>(() => `${props.id}-feedback`);
const textareaDescribedBy = computed<string>(() => [feedbackId.value, props.describedBy].filter((id) => typeof id === "string").join(" "));
const textareaRequired = computed<boolean | "label">(() => (parseBoolean(props.required) ? "label" : false));
const textareaStatus = computed<TextareaStatus | undefined>(() => {
  if (props.status) {
    return props.status;
  }
  switch (isValid.value) {
    case false:
      return "invalid";
    case true:
      return "valid";
  }
  return undefined;
});

defineEmits<{
  (e: "update:model-value", value: string): void;
  (e: "validated", value: ValidationResult): void;
}>();

const rules = computed<ValidationRuleSet>(() => {
  const rules: ValidationRuleSet = {
    required: parseBoolean(props.required),
    minimumLength: parseNumber(props.min),
    maximumLength: parseNumber(props.max),
  };
  return { ...rules, ...props.rules };
});
const { errors, isValid, handleChange, setValue, unbindField } = useField(props.id, {
  focus,
  initialValue: props.modelValue,
  name: props.label ?? props.name,
  rules,
});

watch(
  () => props.modelValue,
  (modelValue) => setValue(modelValue ?? ""),
);

function focus(): void {
  textareaRef.value?.focus();
}
defineExpose({ focus });

onBeforeUnmount(() => {
  if (unbindField) {
    unbindField(props.id);
  }
});
</script>
