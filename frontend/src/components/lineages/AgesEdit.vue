<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AgeInput from "./AgeInput.vue";
import type { AgeCategory, AgesModel } from "@/types/lineages";

const { t } = useI18n();

const props = defineProps<{
  modelValue: AgesModel;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: AgesModel): void;
}>();

function setAge(category: AgeCategory, value?: number): void {
  const ages: AgesModel = { ...props.modelValue };
  switch (category) {
    case "Adolescent":
      ages.adolescent = value ?? undefined;
      break;
    case "Adult":
      ages.adult = value ?? undefined;
      break;
    case "Mature":
      ages.mature = value ?? undefined;
      break;
    case "Venerable":
      ages.venerable = value ?? undefined;
      break;
    default:
      throw new Error(`The age category "${category}" is not supported.`);
  }
  emit("update:model-value", ages);
}
</script>

<template>
  <div>
    <h3>{{ t("lineages.ages.label") }}</h3>
    <div class="row">
      <AgeInput category="Adolescent" class="col-lg-3" :model-value="modelValue.adolescent" @update:model-value="setAge('Adolescent', $event)" />
      <AgeInput
        category="Adult"
        class="col-lg-3"
        :min="modelValue.adolescent ? modelValue.adolescent + 1 : undefined"
        :model-value="modelValue.adult"
        @update:model-value="setAge('Adult', $event)"
      />
      <AgeInput
        category="Mature"
        class="col-lg-3"
        :min="modelValue.adult ? modelValue.adult + 1 : undefined"
        :model-value="modelValue.mature"
        @update:model-value="setAge('Mature', $event)"
      />
      <AgeInput
        category="Venerable"
        class="col-lg-3"
        :min="modelValue.mature ? modelValue.mature + 1 : undefined"
        :model-value="modelValue.venerable"
        @update:model-value="setAge('Venerable', $event)"
      />
    </div>
  </div>
</template>
