<script setup lang="ts">
import { useI18n } from "vue-i18n";

import SizeCategorySelect from "@/components/game/SizeCategorySelect.vue";
import SizeRollInput from "./SizeRollInput.vue";
import type { SizeCategory } from "@/types/game";
import type { SizeModel } from "@/types/lineages";

const { t } = useI18n();

const props = defineProps<{
  modelValue: SizeModel;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: SizeModel): void;
}>();

function setCategory(category: SizeCategory): void {
  const size: SizeModel = { ...props.modelValue, category };
  emit("update:model-value", size);
}
function setRoll(roll?: string): void {
  const size: SizeModel = { ...props.modelValue, roll };
  emit("update:model-value", size);
}
</script>

<template>
  <div>
    <h3>{{ t("game.size.label") }}</h3>
    <div class="row">
      <SizeCategorySelect class="col-lg-6" :model-value="modelValue.category" @update:model-value="setCategory($event ?? 'Medium')" />
      <SizeRollInput class="col-lg-6" :model-value="modelValue.roll" @update:model-value="setRoll" />
    </div>
  </div>
</template>
