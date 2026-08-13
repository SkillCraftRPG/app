<template>
  <TarCard :class="classes">
    <template #title-override>
      <div class="d-flex justify-content-between align-items-start gap-3 w-100">
        <h5 class="card-title">{{ item.name }}</h5>
        <font-awesome-icon v-if="icon" :icon="icon" />
      </div>
    </template>
    <template #subtitle-override>
      <h6 class="card-subtitle mb-2 text-body-secondary">
        {{ category }}
        <span v-if="rarity" class="small">{{ rarity }}</span>
      </h6>
    </template>
    <div v-if="price || weight" class="card-text d-flex justify-content-between align-items-center gap-2 mb-2">
      <div class="text-start">
        <template v-if="price"><font-awesome-icon icon="fas fa-coins" aria-hidden="true" />&nbsp;{{ n(price, "itemPrice") }}</template>
      </div>
      <div v-if="weight" class="text-end"><font-awesome-icon icon="fas fa-weight-hanging" aria-hidden="true" />&nbsp;{{ n(weight, "itemWeight") }}</div>
    </div>
    <div v-if="item.summary" class="card-text">{{ item.summary }}</div>
    <slot></slot>
  </TarCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import TarCard from "@/components/tar/TarCard.vue";
import type { Item } from "@/types/items";
import type { SelectionKind } from "@/types/shared";
import { fromHundredths } from "@/utils/number";

const { parseBoolean } = parsingUtils;
const { n, t } = useI18n();

const props = defineProps<{
  clickable?: boolean | string;
  item: Item;
  selected?: boolean | string;
  selection?: SelectionKind;
}>();

const category = computed<string>(() => t(`items.category.options.${props.item.category}`));
const price = computed<number>(() => fromHundredths(props.item.price) ?? 0);
const rarity = computed<string>(() => (props.item.rarity ? t(`items.rarity.options.${props.item.rarity}`) : ""));
const weight = computed<number>(() => fromHundredths(props.item.weight) ?? 0);

const isSelected = computed<boolean>(() => parseBoolean(props.selected) ?? false);
const classes = computed<string[]>(() => {
  const classes: string[] = [];
  if (parseBoolean(props.clickable)) {
    classes.push("clickable");
  }
  if (isSelected.value) {
    classes.push("border-primary", "bg-primary-subtle");
  }
  return classes;
});
const icon = computed<string>(() => {
  switch (props.selection) {
    case "single":
      return isSelected.value ? "far fa-circle-check" : "far fa-circle";
    case "multiple":
      return isSelected.value ? "far fa-square-check" : "far fa-square";
    default:
      return "";
  }
});
</script>
