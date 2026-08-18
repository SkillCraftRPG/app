<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-2">
      <div class="fs-5">{{ t("customizations.title") }}</div>
      <AddCharacterCustomization v-if="!isReadOnly" :character="character" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
    </div>
    <TarAlert v-if="gifts.length !== disabilities.length" show variant="warning">
      <strong>{{ t("characters.customizations.invalid.lead") }}</strong> {{ t("characters.customizations.invalid.help") }}
    </TarAlert>
    <div v-if="character.customizations.length" class="row">
      <div v-for="gift in gifts" :key="gift.id" class="mb-3" :class="classes">
        <CharacterCustomizationCard
          class="d-flex flex-column h-100"
          :customization="gift"
          :readonly="isReadOnly"
          @click="detail(gift)"
          @remove="remove(gift)"
        />
      </div>
      <div v-for="disability in disabilities" :key="disability.id" class="mb-3" :class="classes">
        <CharacterCustomizationCard
          class="d-flex flex-column h-100"
          :customization="disability"
          :readonly="isReadOnly"
          @click="detail(disability)"
          @remove="remove(disability)"
        />
      </div>
    </div>
    <p v-else>{{ t("characters.customizations.empty") }}</p>
    <CustomizationDetailModal v-if="customization" :customization="customization" ref="detailModal" />
    <RemoveCharacterCustomizationModal
      v-if="customization"
      :character="character"
      :customization="customization"
      ref="removeModal"
      @error="$emit('error', $event)"
      @updated="$emit('updated', $event)"
    />
  </div>
</template>

<script setup lang="ts">
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed, nextTick, ref } from "vue";
import { useI18n } from "vue-i18n";

import AddCharacterCustomization from "./AddCharacterCustomization.vue";
import CharacterCustomizationCard from "./CharacterCustomizationCard.vue";
import CustomizationDetailModal from "@/components/customizations/CustomizationDetailModal.vue";
import RemoveCharacterCustomizationModal from "./RemoveCharacterCustomizationModal.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import type { Character } from "@/types/characters";
import type { Customization } from "@/types/customizations";

const { orderBy } = arrayUtils;
const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  character: Character;
  readonly?: boolean | string;
}>();

defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Character): void;
}>();

const customization = ref<Customization>();
const detailModal = ref<InstanceType<typeof CustomizationDetailModal> | null>(null);
const removeModal = ref<InstanceType<typeof RemoveCharacterCustomizationModal> | null>(null);

const classes = computed<string>(() => {
  switch (props.character.customizations.length) {
    case 1:
    case 2:
      return "col-md-6";
    case 5:
    case 6:
      return "col-md-4";
    default:
      return "col-md-6 col-lg-4 col-xl-3";
  }
});
const disabilities = computed<Customization[]>(() =>
  orderBy(
    props.character.customizations.filter((customization) => customization.kind === "Disability"),
    "name",
  ),
);
const gifts = computed<Customization[]>(() =>
  orderBy(
    props.character.customizations.filter((customization) => customization.kind === "Gift"),
    "name",
  ),
);
const isReadOnly = computed<boolean>(() => parseBoolean(props.readonly) ?? false);

function detail(value: Customization): void {
  customization.value = value;
  nextTick(() => detailModal.value?.open());
}

function remove(value: Customization): void {
  customization.value = value;
  nextTick(() => removeModal.value?.open());
}
</script>
