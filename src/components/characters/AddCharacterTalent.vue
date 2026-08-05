<template>
  <div>
    <TarButton icon="fas fa-plus" size="large" :text="t('actions.add')" @click="open" />
    <TarModal centered :close="t('actions.close')" fade ref="modal" scrollable size="x-large" :title="t('characters.talents.add')">
      <template v-if="step === 'select'">
        <p class="text-body-secondary">{{ t("characters.talents.help") }}</p>
        <section class="row">
          <div class="col-lg-6 col-xl-4">
            <TalentTierSelect class="mb-3" :disabled="parsedTier === 0" :max="parsedTier" v-model="selectedTier" />
          </div>
          <div class="col-lg-6 col-xl-4">
            <MultiplePurchasesSelect class="mb-3" v-model="allowMultiplePurchases" />
          </div>
          <div class="col-lg-6 col-xl-4">
            <SkillSelect class="mb-3" v-model="skill" />
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
              :options="requiredTalents"
              :placeholder="t('all')"
              v-model="requiredTalentId"
            />
          </div>
        </section>
        <section v-if="options.length" class="border-top border-secondary-subtle pt-3">
          <div class="row">
            <div v-for="talent in options" :key="talent.id" class="col-lg-6 col-xl-4 mb-3">
              <TalentCard
                class="d-flex flex-column h-100"
                clickable
                :selected="talent.id === selectedTalent?.id"
                selection="single"
                :talent="talent"
                @click="toggle(talent)"
              />
            </div>
          </div>
          <a v-if="showLimit" href="#" @click="toggleLimit">{{ t(limit ? "actions.showMore" : "actions.showLess") }}</a>
        </section>
        <section v-else class="d-flex flex-column align-items-center justify-content-center text-center flex-grow-1 py-5">
          <font-awesome-icon icon="fas fa-magnifying-glass" class="display-4 text-body-secondary mb-3" aria-hidden="true" />
          <h2 class="h4 mb-2">{{ t("empty.lead") }}</h2>
          <p class="text-body-secondary mb-0">{{ t("empty.help") }}</p>
          <TarButton v-if="hasFilters" class="mt-3" icon="fas fa-arrow-rotate-left" outline :text="t('filters.clear')" variant="secondary" @click="reset" />
        </section>
      </template>
      <template v-else-if="step === 'form'">
        <div>form</div>
      </template>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton v-if="step === 'select'" :disabled="!selectedTalent" icon="fas fa-arrow-right" :text="t('actions.next')" @click="next" />
        <TarButton v-else icon="fas fa-plus" :text="t('actions.add')" />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, ref, watch } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import MultiplePurchasesSelect from "@/components/talents/MultiplePurchasesSelect.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SkillSelect from "@/components/game/SkillSelect.vue";
import TalentCard from "@/components/talents/TalentCard.vue";
import TalentTierSelect from "@/components/talents/TalentTierSelect.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import TarSelect from "@/components/tar/TarSelect.vue";
import type { Talent } from "@/types/talents";
import type { SelectOption } from "@/types/tar/select";

type Step = "select" | "form";

const LIMIT: number = 12;
const { orderBy } = arrayUtils;
const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  acquired: Talent[];
  talents: Talent[];
  tier: number | string;
}>();

const allowMultiplePurchases = ref<boolean>();
const limit = ref<number>(LIMIT);
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const requiredTalentId = ref<string>("");
const search = ref<string>("");
const selectedTalent = ref<Talent>();
const selectedTier = ref<number>();
const skill = ref<string>("");
const step = ref<Step>("select");

const parsedTier = computed<number>(() => parseNumber(props.tier) ?? 0);

const acquiredTalentIds = computed<Set<string>>(() => new Set(props.acquired.map((talent) => talent.id)));
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
    if ((skill.value === "any" && !talent.skill) || (skill.value === "none" && talent.skill) || (skill.value && talent.skill !== skill.value)) {
      return false;
    }
    const searchText: string = search.value.trim().toLocaleLowerCase();
    if (searchText && !talent.name.toLocaleLowerCase().includes(searchText) && talent.summary?.toLowerCase().includes(searchText) !== true) {
      return false;
    }
    if (requiredTalentId.value && talent.requiredTalent?.id !== requiredTalentId.value) {
      return false;
    }
    return true;
  }),
);
const hasFilters = computed<boolean>(() =>
  Boolean(typeof allowMultiplePurchases.value === "boolean" || requiredTalentId.value || search.value || typeof selectedTier.value === "number" || skill.value),
);
const options = computed<Talent[]>(() => {
  const ordered: Talent[] = orderBy(filtered.value, "name");
  return limit.value ? ordered.slice(0, limit.value) : ordered;
});
const requiredTalents = computed<SelectOption[]>(() =>
  orderBy(
    props.acquired.map(({ id, name }) => ({ text: name, value: id })),
    "value",
  ),
);
const showLimit = computed<boolean>(() => options.value.length < filtered.value.length || options.value.length > LIMIT);

function reset(): void {
  allowMultiplePurchases.value = undefined;
  limit.value = LIMIT;
  requiredTalentId.value = "";
  search.value = "";
  selectedTalent.value = undefined;
  if (parsedTier.value > 0) {
    selectedTier.value = undefined;
  }
  skill.value = "";
  step.value = "select";
}

function toggle(talent: Talent): void {
  if (selectedTalent.value?.id === talent.id) {
    selectedTalent.value = undefined;
  } else {
    selectedTalent.value = talent;
  }
}
function toggleLimit(): void {
  limit.value = limit.value ? 0 : LIMIT;
}

function cancel(): void {
  reset();
  modal.value?.hide();
}

function next(): void {
  if (step.value === "select") {
    step.value = "form";
  }
}

function open(): void {
  modal.value?.show();
}

watch(
  parsedTier,
  (parsedTier) => {
    if (parsedTier === 0) {
      selectedTier.value = 0;
    }
  },
  { immediate: true },
);
</script>
