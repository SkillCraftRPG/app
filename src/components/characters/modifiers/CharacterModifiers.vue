<template>
  <div>
    <div class="d-flex gap-2 mb-3">
      <TarButton icon="fas fa-plus" size="large" :text="t('actions.add')" @click="add" />
      <TarButton
        v-if="hasModifiers && hasFilters"
        icon="fas fa-arrow-rotate-left"
        outline
        :text="t('filters.clear')"
        variant="secondary"
        @click="clearFilters"
      />
    </div>
    <template v-if="hasModifiers">
      <div class="row">
        <div class="col-md-6 col-lg-3">
          <SearchInput class="mb-3" v-model="search" />
        </div>
        <div class="col-md-6 col-lg-3">
          <CharacterModifierKindSelect class="mb-3" :model-value="kind" @update:model-value="updateKind" />
        </div>
        <div class="col-md-6 col-lg-3">
          <SkillSelect v-if="kind === 'Skill'" class="mb-3" v-model="target" />
          <StatisticSelect v-else-if="kind === 'Statistic'" class="mb-3" v-model="target" />
          <SpeedKindSelect v-else-if="kind === 'Speed'" class="mb-3" v-model="target" />
          <AttributeSelect v-else class="mb-3" :disabled="kind !== 'Attribute'" v-model="target" />
        </div>
        <div class="col-md-6 col-lg-3">
          <CharacterModifierTypeSelect class="mb-3" v-model="type" />
        </div>
      </div>
      <div v-if="modifiers.length" class="row">
        <div v-for="modifier in modifiers" :key="modifier.id" class="col-md-6 col-lg-4 col-xl-3">
          <CharacterModifier class="mb-3" :modifier="modifier" @edit="edit(modifier)" @remove="remove(modifier)" />
        </div>
      </div>
      <section v-else class="d-flex flex-column align-items-center justify-content-center text-center flex-grow-1 py-5">
        <font-awesome-icon icon="fas fa-magnifying-glass" class="display-4 text-body-secondary mb-3" aria-hidden="true" />
        <h2 class="h4 mb-2">{{ t("empty.lead") }}</h2>
        <p class="text-body-secondary mb-0">{{ t("empty.help") }}</p>
        <TarButton
          v-if="hasFilters"
          class="mt-3"
          icon="fas fa-arrow-rotate-left"
          outline
          :text="t('filters.clear')"
          variant="secondary"
          @click="clearFilters"
        />
      </section>
    </template>
    <p v-else>{{ t("characters.modifiers.empty") }}</p>
    <EditCharacterModifierModal
      :character="character"
      :modifier="modifier"
      ref="editModal"
      @error="$emit('error', $event)"
      @updated="$emit('updated', $event)"
    />
    <RemoveCharacterModifierModal
      v-if="modifier"
      :character="character"
      :modifier="modifier"
      ref="removeModal"
      @error="$emit('error', $event)"
      @updated="$emit('updated', $event)"
    />
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, nextTick, ref } from "vue";
import { useI18n } from "vue-i18n";

import AttributeSelect from "@/components/game/AttributeSelect.vue";
import CharacterModifier from "./CharacterModifier.vue";
import CharacterModifierKindSelect from "./CharacterModifierKindSelect.vue";
import CharacterModifierTypeSelect from "./CharacterModifierTypeSelect.vue";
import EditCharacterModifierModal from "./EditCharacterModifierModal.vue";
import RemoveCharacterModifierModal from "./RemoveCharacterModifierModal.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SkillSelect from "@/components/game/SkillSelect.vue";
import SpeedKindSelect from "@/components/game/SpeedKindSelect.vue";
import StatisticSelect from "@/components/game/StatisticSelect.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { Character, CharacterModifier as CharacterModifierT } from "@/types/characters";
import { translateKind, translateTarget } from "@/utils/modifier";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  character: Character;
}>();

defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Character): void;
}>();

const editModal = ref<InstanceType<typeof EditCharacterModifierModal> | null>(null);
const kind = ref<string>("");
const removeModal = ref<InstanceType<typeof RemoveCharacterModifierModal> | null>(null);
const modifier = ref<CharacterModifierT>();
const search = ref<string>("");
const target = ref<string>("");
const type = ref<string>("");

const hasFilters = computed<boolean>(() => Boolean(search.value || kind.value || target.value || type.value));
const hasModifiers = computed<boolean>(() => props.character.modifiers.length > 0);

type IndexedCharacterModifier = CharacterModifierT & {
  search: string;
  sort: string;
};
const indexed = computed<IndexedCharacterModifier[]>(() =>
  props.character.modifiers.map((modifier) => {
    const target: string = translateTarget(modifier, t);
    const search: string[] = [translateKind(modifier, t), target];
    if (modifier.name) {
      search.push(modifier.name);
    }
    return {
      ...modifier,
      search: search.join(" "),
      sort: modifier.name ?? target,
    };
  }),
);
const modifiers = computed<CharacterModifierT[]>(() =>
  orderBy(
    indexed.value.filter((modifier) => {
      const searchText: string = search.value.trim().toLocaleLowerCase();
      if (searchText && !modifier.search.toLocaleLowerCase().includes(searchText)) {
        return false;
      }
      if ((kind.value && modifier.kind !== kind.value) || (target.value && modifier.target !== target.value)) {
        return false;
      }
      switch (type.value) {
        case "bonus":
          if (modifier.value <= 0) {
            return false;
          }
          break;
        case "penalty":
          if (modifier.value >= 0) {
            return false;
          }
          break;
      }
      return true;
    }),
    "sort",
  ),
);

function add(): void {
  modifier.value = undefined;
  editModal.value?.open();
}
function edit(value: CharacterModifierT): void {
  modifier.value = value;
  editModal.value?.open();
}
function remove(value: CharacterModifierT): void {
  modifier.value = value;
  nextTick(() => removeModal.value?.open());
}

function clearFilters(): void {
  search.value = "";
  kind.value = "";
  target.value = "";
  type.value = "";
}

function updateKind(value: string): void {
  kind.value = value;
  target.value = "";
}
</script>
