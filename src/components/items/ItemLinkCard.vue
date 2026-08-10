<template>
  <LinkCard :subtitle="subtitle" :title="item.name" :to="{ name: 'Item', params: { id: item.id } }">
    <div v-if="price || weight" class="card-text d-flex justify-content-between align-items-center gap-2 mb-2">
      <div class="text-start">
        <template v-if="price"><font-awesome-icon icon="fas fa-coins" aria-hidden="true" />&nbsp;{{ n(price, "itemPrice") }}</template>
      </div>
      <div v-if="weight" class="text-end"><font-awesome-icon icon="fas fa-weight-hanging" aria-hidden="true" />&nbsp;{{ n(weight, "itemWeight") }}</div>
    </div>
    <div v-if="item.summary" class="card-text">{{ item.summary }}</div>
    <StatusBlock :actor="item.updatedBy" class="card-text mt-2 small text-secondary" :date="item.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Item } from "@/types/items";
import { fromHundredths } from "@/utils/number";

const { n, t } = useI18n();

const props = defineProps<{
  item: Item;
}>();

const price = computed<number>(() => fromHundredths(props.item.price) ?? 0);
const subtitle = computed<string>(() => t(`items.category.options.${props.item.category}`));
const weight = computed<number>(() => fromHundredths(props.item.weight) ?? 0);
</script>
