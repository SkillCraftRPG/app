<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-2">
      <div class="fs-5">{{ t("customizations.title") }}</div>
      <TarButton v-if="!isReadOnly" icon="fas fa-plus" size="small" :text="t('actions.add')" @click="add" />
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
    <template v-if="!isReadOnly">
      <AddCharacterCustomizationModal
        :character="character"
        :customizations="customizations"
        ref="addModal"
        @error="$emit('error', $event)"
        @updated="$emit('updated', $event)"
      />
      <RemoveCharacterCustomizationModal
        v-if="customization"
        :character="character"
        :customization="customization"
        ref="removeModal"
        @error="$emit('error', $event)"
        @updated="$emit('updated', $event)"
      />
    </template>
  </div>
</template>

<script setup lang="ts">
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed, nextTick, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import AddCharacterCustomizationModal from "./AddCharacterCustomizationModal.vue";
import CharacterCustomizationCard from "./CharacterCustomizationCard.vue";
import CustomizationDetailModal from "@/components/customizations/CustomizationDetailModal.vue";
import RemoveCharacterCustomizationModal from "./RemoveCharacterCustomizationModal.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { Character } from "@/types/characters";
import type { Customization } from "@/types/customizations";
import type { SearchResults } from "@/types/search";
import { listCustomizations } from "@/api/customizations";

const { orderBy } = arrayUtils;
const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  character: Character;
  readonly?: boolean | string;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Character): void;
}>();

const addModal = ref<InstanceType<typeof AddCharacterCustomizationModal> | null>(null);
const customization = ref<Customization>();
const customizations = ref<Customization[]>([]);
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

function add(): void {
  addModal.value?.open();
}
function detail(value: Customization): void {
  customization.value = value;
  nextTick(() => detailModal.value?.open());
}

function remove(value: Customization): void {
  customization.value = value;
  nextTick(() => removeModal.value?.open());
}

onMounted(async () => {
  try {
    const results: SearchResults<Customization> = await listCustomizations();
    customizations.value = [...results.items];
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>
