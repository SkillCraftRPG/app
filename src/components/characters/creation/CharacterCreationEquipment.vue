<template>
  <form @submit.prevent="handleSubmit(submit)">
    <h2 class="h3">{{ t("characters.creation.equipment.title") }}</h2>
    <LoadingSpinner v-if="isLoading" />
    <TarAlert v-else-if="errors.length" show variant="warning">
      <p>
        <strong>{{ t("characters.creation.equipment.invalid.lead") }}</strong>
      </p>
      <ul>
        <li v-for="error in errors" :key="error">{{ t(`characters.creation.equipment.invalid.${error}`) }}</li>
      </ul>
    </TarAlert>
    <template v-else>
      <p class="text-body-secondary">{{ t("characters.creation.equipment.help") }}</p>
      <div class="row">
        <div class="col-md-6">
          <InputField
            class="mb-3"
            id="starting-wealth"
            :label="t('characters.creation.equipment.wealth')"
            min="0"
            max="999999999"
            :model-value="quantity.toString()"
            step="1"
            type="number"
            @update:model-value="quantity = parseNumber($event) ?? 0"
          >
            <template #append>
              <TarButton icon="fas fa-dice" :text="startingWealth" @click="randomizeQuantity" />
            </template>
          </InputField>
        </div>
      </div>
      <h3 class="h5">{{ t("items.category.options.Currency") }}</h3>
      <p class="text-body-secondary">{{ t("characters.creation.equipment.currency") }}</p>
      <div class="row">
        <div v-for="item in items" :key="item.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
          <ItemCard class="h-100" clickable :item="item" :selected="item.id === currency?.id" selection="single" @click="toggle(item)" />
        </div>
      </div>
    </template>
    <div class="d-flex justify-content-between">
      <div class="d-flex gap-2">
        <TarButton icon="fas fa-xmark" outline :text="t('actions.abandon')" variant="danger" @click="$emit('abandon')" />
        <TarButton icon="fas fa-arrow-left" outline :text="t('actions.previous')" variant="secondary" @click="character.goBack" />
      </div>
      <TarButton :disabled="!canSubmit" icon="fas fa-check" :text="t('actions.complete')" type="submit" />
    </div>
  </form>
</template>

<script setup lang="ts">
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import InputField from "@/components/forms/InputField.vue";
import ItemCard from "@/components/items/ItemCard.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { Item, SearchItemsPayload } from "@/types/items";
import type { SearchResults } from "@/types/search";
import { roll } from "@/utils/random";
import { searchItems } from "@/api/items";
import { useCharacterStore } from "@/stores/character";
import { useForm } from "@/forms";

const character = useCharacterStore();
const { n, t } = useI18n();
const { orderBy } = arrayUtils;
const { parseNumber } = parsingUtils;

const emit = defineEmits<{
  (e: "abandon"): void;
  (e: "complete"): void;
  (e: "error", value: unknown): void;
}>();

const currency = ref<Item>();
const isLoading = ref<boolean>(true);
const items = ref<Item[]>([]);
const quantity = ref<number>(0);

const errors = computed<string[]>(() => {
  const errors: string[] = [];
  if (!character.creation.caste?.wealthRoll) {
    errors.push("caste");
  }
  if (!character.creation.education?.wealthMultiplier) {
    errors.push("education");
  }
  if (!items.value.length) {
    errors.push("item");
  }
  return errors;
});
const canSubmit = computed<boolean>(() => Boolean(errors.value.length || !quantity.value || currency.value));
const startingWealth = computed<string>(() =>
  character.creation.caste?.wealthRoll && character.creation.education?.wealthMultiplier
    ? `${character.creation.caste.wealthRoll} × ${n(character.creation.education.wealthMultiplier, "integer")}`
    : "",
);

function randomizeQuantity(): void {
  quantity.value = roll(character.creation.caste?.wealthRoll ?? "") * (character.creation.education?.wealthMultiplier ?? 0);
}

function toggle(value: Item): void {
  if (currency.value?.id === value.id) {
    currency.value = undefined;
  } else {
    currency.value = value;
  }
}

const { handleSubmit } = useForm();
function submit(): void {
  if (canSubmit.value) {
    character.saveEquipment(quantity.value, currency.value);
    emit("complete");
  }
}

onMounted(async () => {
  try {
    const payload: SearchItemsPayload = {
      category: "Currency",
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<Item> = await searchItems(payload);
    items.value = orderBy(
      results.items.filter((item) => item.price === 100),
      "name",
    );

    if (character.creation.quantity) {
      quantity.value = character.creation.quantity;
    } else {
      randomizeQuantity();
    }

    if (character.creation.currency) {
      currency.value = items.value.find((item) => item.id === character.creation.currency?.id);
    }
    if (!currency.value && items.value.length === 1) {
      currency.value = items.value[0];
    }
  } catch (e: unknown) {
    emit("error", e);
  } finally {
    isLoading.value = false;
  }
});
</script>
