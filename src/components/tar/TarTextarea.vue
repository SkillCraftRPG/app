<template>
  <div>
    <slot v-if="!isFloating" name="label-override">
      <label v-if="label" :for="id" class="form-label">
        {{ label }}
        <slot name="label-required" v-if="isLabelRequired">
          <span class="text-danger">*</span>
        </slot>
      </label>
    </slot>
    <slot name="before"></slot>
    <div v-if="isFloating" class="form-floating">
      <slot>
        <textarea
          :aria-describedby="describedBy"
          :class="classes"
          :cols="parseNumber(cols)"
          :disabled="isDisabled"
          :id="id"
          :maxlength="parseNumber(props.max) || undefined"
          :minlength="parseNumber(props.min) || undefined"
          :name="name"
          :placeholder="placeholder"
          :readonly="isReadonly"
          ref="textareaRef"
          :required="isRequired"
          :style="{ height }"
          :value="modelValue"
          @input="$emit('update:model-value', ($event.target as HTMLTextAreaElement).value)"
        >
        </textarea>
      </slot>
      <slot name="label-override">
        <label :for="id">
          {{ label }}
          <slot name="label-required" v-if="isLabelRequired">
            <span class="text-danger">*</span>
          </slot>
        </label>
      </slot>
    </div>
    <slot v-else>
      <textarea
        :aria-describedby="describedBy"
        :class="classes"
        :cols="parseNumber(cols)"
        :disabled="isDisabled"
        :id="id"
        :maxlength="parseNumber(props.max) || undefined"
        :minlength="parseNumber(props.min) || undefined"
        :name="name"
        :placeholder="placeholder"
        :readonly="isReadonly"
        ref="textareaRef"
        :required="isRequired"
        :rows="parseNumber(rows)"
        :value="modelValue"
        @input="$emit('update:model-value', ($event.target as HTMLTextAreaElement).value)"
      >
      </textarea>
    </slot>
    <slot name="after"></slot>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { parsingUtils } from "logitar-js";

import type { TextareaOptions } from "@/types/tar/textarea";

const { parseBoolean, parseNumber } = parsingUtils;

const props = defineProps<TextareaOptions>();
const textareaRef = ref<HTMLTextAreaElement>();

const isDisabled = computed<boolean>(() => parseBoolean(props.disabled) ?? false);
const isFloating = computed<boolean>(() => parseBoolean(props.floating) ?? false);
const isReadonly = computed<boolean>(() => parseBoolean(props.readonly) ?? false);
const isRequired = computed<boolean>(() => parseBoolean(props.required) ?? false);
const isLabelRequired = computed<boolean>(() => isRequired.value || (typeof props.required === "string" && props.required.trim().toLowerCase() === "label"));

const classes = computed<string[]>(() => {
  const classes: string[] = [];
  if (isReadonly.value && parseBoolean(props.plaintext)) {
    classes.push("form-control-plaintext");
  } else {
    classes.push("form-control");
  }
  switch (props.size) {
    case "large":
      classes.push("form-control-lg");
      break;
    case "small":
      classes.push("form-control-sm");
      break;
  }
  if (props.status) {
    classes.push(`is-${props.status}`);
  }
  return classes;
});
const height = computed<string | undefined>(() => {
  const rows = parseNumber(props.rows);
  return rows ? `${rows * 1.5}rem` : undefined;
});

/**
 * Focuses the textarea element.
 */
function focus(): void {
  textareaRef.value?.focus();
}
defineExpose({ focus });

defineEmits<{
  /**
   * The textarea value has been updated.
   */
  (e: "update:model-value", value?: string): void;
}>();
</script>
