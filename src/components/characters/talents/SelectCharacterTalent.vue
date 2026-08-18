<template>
  <div>
    <p class="text-body-secondary">{{ t("characters.talents.select") }}</p>
    <section class="row">
      <div class="col-lg-6 col-xl-4">
        <TalentTierSelect class="mb-3" :disabled="!context.tier" :max="context.tier" v-model="selectedTier" />
      </div>
      <div class="col-lg-6 col-xl-4">
        <MultiplePurchasesSelect class="mb-3" v-model="allowMultiplePurchases" />
      </div>
      <div class="col-lg-6 col-xl-4">
        <SkillSelect class="mb-3" extended v-model="skill" />
      </div>
      <div class="col-lg-6">
        <SearchInput class="mb-3" v-model="search" />
      </div>
      <div class="col-lg-6">
        <TarSelect
          class="mb-3"
          :disabled="!requiredTalents.length"
          floating
          id="required-talent"
          :label="t('talents.required')"
          :model-value="requiredTalent?.id"
          :options="requiredTalents"
          :placeholder="t('all')"
          @update:model-value="selectRequiredTalent"
        />
      </div>
    </section>
    <section v-if="options.length" class="border-top border-secondary-subtle pt-3">
      <div class="row">
        <div v-for="talent in options" :key="talent.id" class="col-lg-6 col-xl-4 mb-3">
          <TalentCard
            class="d-flex flex-column h-100"
            clickable
            :selected="talent.id === selected?.id"
            selection="single"
            :talent="talent"
            @click="$emit('toggle', talent)"
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
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import MultiplePurchasesSelect from "@/components/talents/MultiplePurchasesSelect.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SkillSelect from "@/components/game/SkillSelect.vue";
import TalentCard from "@/components/talents/TalentCard.vue";
import TalentTierSelect from "@/components/talents/TalentTierSelect.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarSelect from "@/components/tar/TarSelect.vue";
import type { CharacterTalentContext } from "@/types/characters";
import type { SelectOption } from "@/types/tar/select";
import type { Talent } from "@/types/talents";

const LIMIT: number = 12;
const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  context: CharacterTalentContext;
  selected?: Talent;
  talents: Talent[];
}>();

defineEmits<{
  (e: "toggle", value: Talent): void;
}>();

const allowMultiplePurchases = ref<boolean>();
const limit = ref<number>(LIMIT);
const requiredTalent = ref<Talent>();
const search = ref<string>("");
const selectedTier = ref<number>();
const skill = ref<string>("");

const acquiredTalentIds = computed<Set<string>>(() => new Set(props.context.talents.map((acquired) => acquired.talent.id)));
const filtered = computed<Talent[]>(() =>
  props.talents.filter((talent) => {
    if (acquiredTalentIds.value.has(talent.id) && !talent.allowMultiplePurchases) {
      return false;
    }
    if (talent.requiredTalent && !acquiredTalentIds.value.has(talent.requiredTalent.id)) {
      return false;
    }
    if (typeof selectedTier.value === "number" && talent.tier !== selectedTier.value) {
      return false;
    }
    if (typeof allowMultiplePurchases.value === "boolean" && talent.allowMultiplePurchases !== allowMultiplePurchases.value) {
      return false;
    }
    switch (skill.value) {
      case "any":
        if (!talent.skill) {
          return false;
        }
        break;
      case "none":
        if (talent.skill) {
          return false;
        }
        break;
      default:
        if (skill.value && skill.value !== talent.skill) {
          return false;
        }
        break;
    }
    const searchText: string = search.value.trim().toLocaleLowerCase();
    if (searchText && !talent.name.toLocaleLowerCase().includes(searchText) && talent.summary?.toLocaleLowerCase().includes(searchText) !== true) {
      return false;
    }
    if (requiredTalent.value && talent.requiredTalent?.id !== requiredTalent.value.id) {
      return false;
    }
    return true;
  }),
);
const hasFilters = computed<boolean>(() =>
  Boolean(typeof allowMultiplePurchases.value === "boolean" || requiredTalent.value || search.value || typeof selectedTier.value === "number" || skill.value),
);
const options = computed<Talent[]>(() => {
  const ordered: Talent[] = orderBy(filtered.value, "name");
  return limit.value ? ordered.slice(0, limit.value) : ordered;
});

const requiredTalents = computed<SelectOption[]>(() =>
  orderBy(
    props.context.talents.map(({ talent }) => ({ text: talent.name, value: talent.id })),
    "value",
  ),
);
const showLimit = computed<boolean>(() => options.value.length < filtered.value.length || options.value.length > LIMIT);

function clearFilters(): void {
  allowMultiplePurchases.value = undefined;
  limit.value = LIMIT;
  requiredTalent.value = undefined;
  search.value = "";
  skill.value = "";

  if (props.context.tier) {
    selectedTier.value = undefined;
  }
}

function selectRequiredTalent(id?: string): void {
  requiredTalent.value = props.context.talents.find(({ talent }) => talent.id === id)?.talent;
}

function toggleLimit(): void {
  limit.value = limit.value ? 0 : LIMIT;
}

watch(
  () => props.context,
  (context) => {
    if (!context.tier) {
      selectedTier.value = 0;
    }
  },
  { deep: true, immediate: true },
);

defineExpose({ clearFilters });
</script>
