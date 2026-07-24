<template>
  <div>
    <p>{{ t("account.dateOfBirth.label") }}</p>
    <div class="row">
      <div class="col">
        <FormInput
          :id="`${id}-day`"
          :label="t('account.dateOfBirth.day')"
          :model-value="day ? day.toString() : ''"
          :required="isRequired"
          :rules="dayRules"
          step="1"
          type="number"
          @update:model-value="updateDay"
        />
      </div>
      <div class="col">
        <FormSelect
          :id="`${id}-month`"
          :label="t('account.dateOfBirth.month.label')"
          :model-value="month"
          :options="monthOptions"
          :placeholder="t('account.dateOfBirth.month.placeholder')"
          :required="isRequired"
          @update:model-value="updateMonth"
        />
      </div>
      <div class="col">
        <FormInput
          :id="`${id}-year`"
          :label="t('account.dateOfBirth.year')"
          :model-value="year ? year.toString() : ''"
          :required="isRequired"
          :rules="yearRules"
          step="1"
          type="number"
          @update:model-value="updateYear"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { ValidationRuleSet } from "logitar-validation";
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import FormSelect from "@/components/forms/FormSelect.vue";
import type { SelectOption } from "@/types/tar/select";

const { orderBy } = arrayUtils;
const { parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    modelValue?: Date;
  }>(),
  {
    id: "date-of-birth",
  },
);

const emit = defineEmits<{
  (e: "update:model-value", value: Date | null): void;
}>();

const now = new Date();
const min = new Date(now.getFullYear() - 100, now.getMonth(), now.getDate());
const max = new Date(now.getFullYear() - 18, now.getMonth(), now.getDate());

const day = ref<number>(props.modelValue?.getDate() ?? 0);
const month = ref<string>(props.modelValue ? (props.modelValue.getMonth() + 1).toString().padStart(2, "0") : "");
const year = ref<number>(props.modelValue?.getFullYear() ?? 0);

const isRequired = computed<boolean>(() => Boolean(day.value) || Boolean(month.value) || Boolean(year.value));
const maxMonth = computed<string>(() => (year.value === max.getFullYear() ? (max.getMonth() + 1).toString().padStart(2, "0") : "12"));
const maxDay = computed<number>(() => {
  if (year.value === max.getFullYear() && month.value === maxMonth.value) {
    return max.getDate();
  }
  const parsedMonth: number | undefined = parseNumber(month.value);
  if (parsedMonth && year.value) {
    return new Date(Date.UTC(year.value, parsedMonth, 0)).getUTCDate();
  }
  return 31;
});

const dayRules = computed<ValidationRuleSet>(() => ({
  required: isRequired.value,
  minimumValue: isRequired.value ? 1 : undefined,
  maximumValue: maxDay.value,
}));
const monthOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("account.dateOfBirth.month.options"))).map(([value, text]) => ({
      text,
      value,
      disabled: value > maxMonth.value,
    })),
    "value",
  ),
);
const yearRules = computed<ValidationRuleSet>(() => ({
  required: isRequired.value,
  minimumValue: isRequired.value ? min.getFullYear() : undefined,
  maximumValue: max.getFullYear(),
}));

function updateModelValue(): void {
  let date: Date | null = null;
  if (day.value && month.value && year.value) {
    date = new Date(year.value, (parseNumber(month.value) || 1) - 1, day.value);
    if (date < min || date > max) {
      date = null;
    }
  }
  emit("update:model-value", date);
}
function updateDay(value: string | undefined): void {
  day.value = parseNumber(value) ?? 0;
  updateModelValue();
}
function updateMonth(value: string | undefined): void {
  month.value = value ?? "";
  if (day.value > maxDay.value) {
    day.value = 0;
  }
  updateModelValue();
}
function updateYear(value: string | undefined): void {
  year.value = parseNumber(value) ?? 0;
  if (month.value > maxMonth.value) {
    month.value = "";
  }
  if (day.value > maxDay.value) {
    day.value = 0;
  }
  updateModelValue();
}

// TODO(fpion): number inputs should be text, with `inputmode="numeric"` and maxlength.
</script>
