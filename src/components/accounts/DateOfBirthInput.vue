<template>
  <div>
    <p>{{ t("account.dateOfBirth.label") }}</p>
    <div class="row">
      <!-- TODO(fpion): we should change number inputs to text, and have `inputmode="numeric"` instead with a maxlength. -->
      <div class="col">
        <FormInput
          floating
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
        <TarSelect
          floating
          :id="`${id}-month`"
          :label="t('account.dateOfBirth.month.label')"
          :model-value="month"
          :options="monthOptions"
          :placeholder="t('account.dateOfBirth.month.placeholder')"
          :required="isRequired"
          :status="monthStatus"
          @update:model-value="updateMonth"
        />
      </div>
      <div class="col">
        <FormInput
          floating
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
import TarSelect from "@/components/tar/TarSelect.vue";
import type { SelectOption, SelectStatus } from "@/types/tar/select";

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
const lastDayInMonth = computed<number | undefined>(() => {
  const parsedMonth: number | undefined = parseNumber(month.value);
  if (parsedMonth && year.value) {
    return new Date(Date.UTC(year.value, parsedMonth, 0)).getUTCDate();
  }
  return 31;
});

const dayRules = computed<ValidationRuleSet>(() => ({
  required: isRequired.value,
  minimumValue: isRequired.value ? 1 : undefined,
  maximumValue: lastDayInMonth.value, // TODO(fpion): not true, user needs to be 18+!
}));
const monthStatus = computed<SelectStatus | undefined>(() => {
  if (!isRequired.value) {
    return undefined;
  }
  return Boolean(month.value) ? "valid" : "invalid";
});
const monthOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("account.dateOfBirth.month.options"))).map(([value, text]) => ({ text, value })),
    "value",
  ),
);
const yearRules = computed<ValidationRuleSet>(() => ({
  required: isRequired.value,
  minimumValue: isRequired.value ? min.getFullYear() : undefined,
  maximumValue: max.getFullYear(),
}));

function updateModelValue(): void {
  const date = new Date(year.value, (parseNumber(month.value) || 1) - 1, day.value);
  const isValid: boolean = date >= min && date <= max;
  emit("update:model-value", isValid ? date : null);
}
function updateDay(value: string | undefined): void {
  day.value = parseNumber(value) ?? 0;
  updateModelValue();
}
function updateMonth(value: string | undefined): void {
  month.value = value ?? "";
  updateModelValue();
}
function updateYear(value: string | undefined): void {
  year.value = parseNumber(value) ?? 0;
  updateModelValue();
}
</script>
