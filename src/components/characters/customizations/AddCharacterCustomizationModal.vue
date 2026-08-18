<template>
  <TarModal centered :close="t('actions.close')" fade scrollable ref="modal" size="x-large" :title="t('characters.customizations.add')">
    <section v-if="customizations.length" class="row">
      <div class="col-md-6">
        <SearchInput class="mb-3" v-model="search" />
      </div>
      <div class="col-md-6">
        <CustomizationKindSelect class="mb-3" v-model="kind" />
      </div>
    </section>
    <section v-if="options.length" class="border-top border-secondary-subtle pt-3">
      <div class="row">
        <div v-for="customization in options" :key="customization.id" class="col-lg-6 col-xl-4 mb-3">
          <CustomizationCard
            class="d-flex flex-column h-100"
            clickable
            :customization="customization"
            :selected="customization.id === selected?.id"
            selection="single"
            @click="toggle(customization)"
          />
        </div>
      </div>
      <a v-if="showLimit" href="#" @click="toggleLimit">{{ t(limit ? "actions.showMore" : "actions.showLess") }}</a>
    </section>
    <section v-else class="d-flex flex-column align-items-center justify-content-center text-center flex-grow-1 py-5">
      <font-awesome-icon icon="fas fa-magnifying-glass" class="display-4 text-body-secondary mb-3" aria-hidden="true" />
      <h2 class="h4 mb-2">{{ t("empty.lead") }}</h2>
      <p class="text-body-secondary mb-0">{{ t("empty.help") }}</p>
      <TarButton v-if="hasFilters" class="mt-3" icon="fas fa-arrow-rotate-left" outline :text="t('filters.clear')" variant="secondary" @click="clearFilters" />
    </section>
    <template #footer>
      <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
      <TarButton :disabled="isLoading || !selected" icon="fas fa-plus" :loading="isLoading" :status="t('loading')" :text="t('actions.add')" @click="confirm" />
    </template>
  </TarModal>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import CustomizationCard from "@/components/customizations/CustomizationCard.vue";
import CustomizationKindSelect from "@/components/customizations/CustomizationKindSelect.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { Character } from "@/types/characters";
import type { Customization } from "@/types/customizations";
import type { SearchResults } from "@/types/search";
import { addCharacterCustomization } from "@/api/characters";
import { listCustomizations } from "@/api/customizations";

const LIMIT: number = 12;
const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  character: Character;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Character): void;
}>();

const customizations = ref<Customization[]>([]);
const isLoading = ref<boolean>(false);
const kind = ref<string>("");
const limit = ref<number>(LIMIT);
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const search = ref<string>("");
const selected = ref<Customization>();

const exclude = computed<Set<string>>(() => new Set(props.character.customizations.map((customization) => customization.id)));
const filtered = computed<Customization[]>(() =>
  customizations.value.filter((customization) => {
    if (exclude.value.has(customization.id)) {
      return false;
    }
    if (kind.value && customization.kind !== kind.value) {
      return false;
    }
    const searchText: string = search.value.trim().toLocaleLowerCase();
    if (
      searchText &&
      !customization.name.toLocaleLowerCase().includes(searchText) &&
      customization.summary?.toLocaleLowerCase().includes(searchText) !== true
    ) {
      return false;
    }
    return true;
  }),
);
const hasFilters = computed<boolean>(() => Boolean(kind.value || search.value));
const options = computed<Customization[]>(() => {
  const ordered: Customization[] = orderBy(filtered.value, "name");
  return limit.value ? ordered.slice(0, limit.value) : ordered;
});
const showLimit = computed<boolean>(() => options.value.length < filtered.value.length || options.value.length > LIMIT);

function clearFilters(): void {
  kind.value = "";
  limit.value = LIMIT;
  search.value = "";
  selected.value = undefined;
}

function toggle(customization: Customization): void {
  if (selected.value?.id === customization.id) {
    selected.value = undefined;
  } else {
    selected.value = customization;
  }
}

function toggleLimit(): void {
  limit.value = limit.value ? 0 : LIMIT;
}

function cancel(): void {
  clearFilters();
  modal.value?.hide();
}

async function confirm(): Promise<void> {
  if (!isLoading.value && selected.value) {
    isLoading.value = true;
    try {
      const character: Character = await addCharacterCustomization(props.character.id, selected.value.id);
      emit("updated", character);
      clearFilters();
      modal.value?.hide();
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

async function open(): Promise<void> {
  modal.value?.show();
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const results: SearchResults<Customization> = await listCustomizations();
      customizations.value = [...results.items];
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
defineExpose({ open });
</script>
