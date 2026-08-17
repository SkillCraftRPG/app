<template>
  <form @submit.prevent="handleSubmit(submit)">
    <section>
      <h3 class="h5">{{ t("lineages.physical.speeds.lead") }}</h3>
      <p class="text-body-secondary">{{ t("lineages.physical.speeds.help") }}</p>
      <div class="row">
        <div v-for="kind in SPEED_KINDS" :key="kind" class="col-md-6 col-lg-4 col-xl-fifth">
          <SpeedField
            class="mb-3"
            :id="camelCase(kind)"
            :label="`game.speed.kind.options.${kind}`"
            :min="kind === 'Fly' && hover ? 1 : 0"
            v-model="speeds[camelCase(kind)]"
          >
            <template v-if="kind === 'Fly'" #after>
              <TarCheckbox id="hover" :label="t('game.speed.hover')" switch v-model="hover" />
            </template>
          </SpeedField>
        </div>
      </div>
    </section>
    <section>
      <h3 class="h5">{{ t("lineages.physical.size") }}</h3>
      <div class="row">
        <div class="col-md-6">
          <SizeCategoryField class="mb-3" v-model="sizeCategory" />
        </div>
        <div class="col-md-6">
          <HeightRollField class="mb-3" v-model="height" />
        </div>
      </div>
    </section>
    <section>
      <h3 class="h5">{{ t("lineages.physical.weight.label") }}</h3>
      <p class="text-body-secondary">{{ t("lineages.physical.weight.help") }}</p>
      <div class="row">
        <div class="col-md-6 col-lg-4 col-xl-fifth">
          <WeightRollField class="mb-3" id="malnutrition" label="lineages.physical.weight.malnutrition" v-model="malnutrition" />
        </div>
        <div class="col-md-6 col-lg-4 col-xl-fifth">
          <WeightRollField class="mb-3" id="skinny" label="lineages.physical.weight.skinny" v-model="skinny" />
        </div>
        <div class="col-md-6 col-lg-4 col-xl-fifth">
          <WeightRollField class="mb-3" id="normal" label="lineages.physical.weight.normal" v-model="normal" />
        </div>
        <div class="col-md-6 col-lg-4 col-xl-fifth">
          <WeightRollField class="mb-3" id="overweight" label="lineages.physical.weight.overweight" v-model="overweight" />
        </div>
        <div class="col-md-6 col-lg-4 col-xl-fifth">
          <WeightRollField class="mb-3" id="obese" label="lineages.physical.weight.obese" v-model="obese" />
        </div>
      </div>
    </section>
    <section>
      <h3 class="h5">{{ t("lineages.physical.age.title") }}</h3>
      <p class="text-body-secondary">{{ t("lineages.physical.age.help") }}</p>
      <div class="row">
        <div class="col-md-6 col-lg-3">
          <AgeField class="mb-3" id="teenager" label="lineages.physical.age.teenager" :min="isAgeRequired ? 1 : 0" v-model="teenager" />
        </div>
        <div class="col-md-6 col-lg-3">
          <AgeField class="mb-3" id="adult" label="lineages.physical.age.adult" :min="isAgeRequired ? teenager + 1 : 0" v-model="adult" />
        </div>
        <div class="col-md-6 col-lg-3">
          <AgeField class="mb-3" id="mature" label="lineages.physical.age.mature" :min="isAgeRequired ? adult + 1 : 0" v-model="mature" />
        </div>
        <div class="col-md-6 col-lg-3">
          <AgeField class="mb-3" id="venerable" label="lineages.physical.age.venerable" :min="isAgeRequired ? mature + 1 : 0" v-model="venerable" />
        </div>
      </div>
    </section>
    <div class="d-flex justify-content-end mb-3">
      <TarButton
        :disabled="!hasChanges || isLoading"
        icon="fas fa-floppy-disk"
        :loading="isLoading"
        size="large"
        :status="t('loading')"
        :text="t('actions.save')"
        type="submit"
      />
    </div>
  </form>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import AgeField from "./AgeField.vue";
import HeightRollField from "@/components/lineages/HeightRollField.vue";
import SizeCategoryField from "@/components/game/SizeCategoryField.vue";
import SpeedField from "./SpeedField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarCheckbox from "@/components/tar/TarCheckbox.vue";
import WeightRollField from "./WeightRollField.vue";
import type { Lineage, UpdateLineagePayload } from "@/types/lineages";
import type { SizeCategory, SpeedKind } from "@/types/game";
import { SPEED_KINDS, camelCase } from "@/utils/game";
import { updateLineage } from "@/api/lineages";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  lineage: Lineage;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Lineage): void;
}>();

const adult = ref<number>(0);
const height = ref<string>("");
const hover = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const malnutrition = ref<string>("");
const mature = ref<number>(0);
const normal = ref<string>("");
const obese = ref<string>("");
const overweight = ref<string>("");
const sizeCategory = ref<SizeCategory>("Medium");
const skinny = ref<string>("");
const speeds = reactive<Record<Uncapitalize<SpeedKind>, number>>(
  Object.fromEntries(SPEED_KINDS.map((kind) => [camelCase(kind), 0])) as Record<Uncapitalize<SpeedKind>, number>,
);
const teenager = ref<number>(0);
const venerable = ref<number>(0);

const hasChanges = computed<boolean>(
  () =>
    SPEED_KINDS.some((kind) => (props.lineage.speeds[camelCase(kind)] ?? 0) !== speeds[camelCase(kind)]) ||
    props.lineage.speeds.hover !== hover.value ||
    props.lineage.size.category !== sizeCategory.value ||
    (props.lineage.size.height ?? "") !== height.value ||
    (props.lineage.weight.malnutrition ?? "") !== malnutrition.value ||
    (props.lineage.weight.skinny ?? "") !== skinny.value ||
    (props.lineage.weight.normal ?? "") !== normal.value ||
    (props.lineage.weight.overweight ?? "") !== overweight.value ||
    (props.lineage.weight.obese ?? "") !== obese.value ||
    (props.lineage.age.teenager ?? 0) !== teenager.value ||
    (props.lineage.age.adult ?? 0) !== adult.value ||
    (props.lineage.age.mature ?? 0) !== mature.value ||
    (props.lineage.age.venerable ?? 0) !== venerable.value,
);
const isAgeRequired = computed<boolean>(() => Boolean(teenager.value || adult.value || mature.value || venerable.value));

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: UpdateLineagePayload = {
        speeds: {
          ...Object.fromEntries(SPEED_KINDS.map((kind) => [camelCase(kind), speeds[camelCase(kind)] || undefined])),
          hover: hover.value,
        },
        size: {
          category: sizeCategory.value,
          height: height.value,
        },
        weight: {
          malnutrition: malnutrition.value,
          skinny: skinny.value,
          normal: normal.value,
          overweight: overweight.value,
          obese: obese.value,
        },
        age: {
          teenager: teenager.value || undefined,
          adult: adult.value || undefined,
          mature: mature.value || undefined,
          venerable: venerable.value || undefined,
        },
      };
      const lineage: Lineage = await updateLineage(props.lineage.id, payload);
      reinitialize();
      emit("updated", lineage);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.lineage,
  (lineage) => {
    for (const kind of SPEED_KINDS) {
      speeds[camelCase(kind)] = lineage.speeds[camelCase(kind)] ?? 0;
    }
    hover.value = lineage.speeds.hover;
    sizeCategory.value = lineage.size.category;
    height.value = lineage.size.height ?? "";
    malnutrition.value = lineage.weight.malnutrition ?? "";
    skinny.value = lineage.weight.skinny ?? "";
    normal.value = lineage.weight.normal ?? "";
    overweight.value = lineage.weight.overweight ?? "";
    obese.value = lineage.weight.obese ?? "";
    teenager.value = lineage.age.teenager ?? 0;
    adult.value = lineage.age.adult ?? 0;
    mature.value = lineage.age.mature ?? 0;
    venerable.value = lineage.age.venerable ?? 0;
  },
  { deep: true, immediate: true },
);
</script>
