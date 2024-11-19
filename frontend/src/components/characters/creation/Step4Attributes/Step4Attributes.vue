<script setup lang="ts">
import { TarButton, TarCard } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import type { Attribute } from "@/types/game";
import type { AttributeBonusesModel, LineageModel } from "@/types/lineages";
import type { Step1, Step2, Step3, Step4 } from "@/types/characters";
import { calculateModifier } from "@/helpers/gameUtils";
import { useCharacterStore } from "@/stores/character";

const character = useCharacterStore();
const { orderBy } = arrayUtils;
const { t } = useI18n();

type AttributeRow = {
  attribute: Attribute;
  text: string;
  base: number;
  lineage: number;
  personality: number;
  aspects: number;
  score: number;
  modifier: number;
};

const agility = ref<number>(8);
const best = ref<Attribute>();
const coordination = ref<number>(8);
const extra = ref<Attribute[]>([]);
const intellect = ref<number>(8);
const optional = ref<Attribute[]>([]);
const presence = ref<number>(8);
const sensitivity = ref<number>(8);
const spirit = ref<number>(8);
const vigor = ref<number>(8);
const worst = ref<Attribute>();

function calculateAspectBonus(attribute: Attribute): number {
  return 0; // TODO(fpion): implement
}

const isCompleted = computed<boolean>(
  () => remainingPoints.value === 0 && requiredExtra.value === 0 && Boolean(best.value) && Boolean(worst.value) && optional.value.length === 2,
);
const lineageAttributes = computed<AttributeBonusesModel>(() => {
  const step1: Step1 | undefined = character.creation.step1;
  if (!step1) {
    return { agility: 0, coordination: 0, intellect: 0, presence: 0, sensitivity: 0, spirit: 0, vigor: 0, extra: 0 };
  }
  const lineageAttributes: AttributeBonusesModel = step1.species.attributes;
  const nation: LineageModel | undefined = step1.nation;
  if (nation) {
    lineageAttributes.agility += nation.attributes.agility;
    lineageAttributes.coordination += nation.attributes.coordination;
    lineageAttributes.intellect += nation.attributes.intellect;
    lineageAttributes.presence += nation.attributes.presence;
    lineageAttributes.sensitivity += nation.attributes.sensitivity;
    lineageAttributes.spirit += nation.attributes.spirit;
    lineageAttributes.vigor += nation.attributes.vigor;
    lineageAttributes.extra += nation.attributes.extra;
  }
  return lineageAttributes;
});
const remainingPoints = computed<number>(
  () => 57 - agility.value - coordination.value - intellect.value - presence.value - sensitivity.value - spirit.value - vigor.value,
);
const requiredExtra = computed<number>(() => lineageAttributes.value.extra - extra.value.length);
const rows = computed<AttributeRow[]>(() => {
  const step2: Step2 | undefined = character.creation.step2;
  const rows: AttributeRow[] = [
    {
      attribute: "Agility",
      text: t("game.attributes.Agility"),
      base: agility.value,
      lineage: lineageAttributes.value.agility + (extra.value.includes("Agility") ? 1 : 0),
      personality: step2?.personality.attribute === "Agility" ? 1 : 0,
      aspects: calculateAspectBonus("Agility"),
      score: 0,
      modifier: 0,
    },
    {
      attribute: "Coordination",
      text: t("game.attributes.Coordination"),
      base: coordination.value,
      lineage: lineageAttributes.value.coordination + (extra.value.includes("Coordination") ? 1 : 0),
      personality: step2?.personality.attribute === "Coordination" ? 1 : 0,
      aspects: calculateAspectBonus("Coordination"),
      score: 0,
      modifier: 0,
    },
    {
      attribute: "Intellect",
      text: t("game.attributes.Intellect"),
      base: intellect.value,
      lineage: lineageAttributes.value.intellect + (extra.value.includes("Intellect") ? 1 : 0),
      personality: step2?.personality.attribute === "Intellect" ? 1 : 0,
      aspects: calculateAspectBonus("Intellect"),
      score: 0,
      modifier: 0,
    },
    {
      attribute: "Presence",
      text: t("game.attributes.Presence"),
      base: presence.value,
      lineage: lineageAttributes.value.presence + (extra.value.includes("Presence") ? 1 : 0),
      personality: step2?.personality.attribute === "Presence" ? 1 : 0,
      aspects: calculateAspectBonus("Presence"),
      score: 0,
      modifier: 0,
    },
    {
      attribute: "Sensitivity",
      text: t("game.attributes.Sensitivity"),
      base: sensitivity.value,
      lineage: lineageAttributes.value.sensitivity + (extra.value.includes("Sensitivity") ? 1 : 0),
      personality: step2?.personality.attribute === "Sensitivity" ? 1 : 0,
      aspects: calculateAspectBonus("Sensitivity"),
      score: 0,
      modifier: 0,
    },
    {
      attribute: "Spirit",
      text: t("game.attributes.Spirit"),
      base: spirit.value,
      lineage: lineageAttributes.value.spirit + (extra.value.includes("Spirit") ? 1 : 0),
      personality: step2?.personality.attribute === "Spirit" ? 1 : 0,
      aspects: calculateAspectBonus("Spirit"),
      score: 0,
      modifier: 0,
    },
    {
      attribute: "Vigor",
      text: t("game.attributes.Vigor"),
      base: vigor.value,
      lineage: lineageAttributes.value.vigor + (extra.value.includes("Vigor") ? 1 : 0),
      personality: step2?.personality.attribute === "Vigor" ? 1 : 0,
      aspects: calculateAspectBonus("Vigor"),
      score: 0,
      modifier: 0,
    },
  ];
  rows.forEach((row) => {
    row.score = row.base + row.lineage + row.personality + row.aspects;
    row.modifier = calculateModifier(row.score);
  });
  return orderBy(rows, "text");
});

defineEmits<{
  (e: "error", value: unknown): void;
}>();

function decreaseBase(row: AttributeRow): void {
  switch (row.attribute) {
    case "Agility":
      agility.value--;
      break;
    case "Coordination":
      coordination.value--;
      break;
    case "Intellect":
      intellect.value--;
      break;
    case "Presence":
      presence.value--;
      break;
    case "Sensitivity":
      sensitivity.value--;
      break;
    case "Spirit":
      spirit.value--;
      break;
    case "Vigor":
      vigor.value--;
      break;
    default:
      throw new Error(`The attribute "${row.attribute}" is not supported.`);
  }
}
function increaseBase(row: AttributeRow): void {
  switch (row.attribute) {
    case "Agility":
      agility.value++;
      break;
    case "Coordination":
      coordination.value++;
      break;
    case "Intellect":
      intellect.value++;
      break;
    case "Presence":
      presence.value++;
      break;
    case "Sensitivity":
      sensitivity.value++;
      break;
    case "Spirit":
      spirit.value++;
      break;
    case "Vigor":
      vigor.value++;
      break;
    default:
      throw new Error(`The attribute "${row.attribute}" is not supported.`);
  }
}

const { handleSubmit } = useForm();
const onSubmit = handleSubmit(() => {
  if (best.value && worst.value) {
    const payload: Step4 = {
      attributes: {
        agility: agility.value,
        coordination: coordination.value,
        intellect: intellect.value,
        presence: presence.value,
        sensitivity: sensitivity.value,
        spirit: spirit.value,
        vigor: vigor.value,
        best: best.value,
        worst: worst.value,
        optional: optional.value,
        extra: extra.value,
      },
    };
    character.setStep4(payload);
  }
});

onMounted(() => {
  try {
    const step4: Step4 | undefined = character.creation.step4;
    if (step4) {
      agility.value = step4.attributes.agility;
      coordination.value = step4.attributes.coordination;
      intellect.value = step4.attributes.intellect;
      presence.value = step4.attributes.presence;
      sensitivity.value = step4.attributes.sensitivity;
      spirit.value = step4.attributes.spirit;
      vigor.value = step4.attributes.vigor;
      best.value = step4.attributes.best;
      worst.value = step4.attributes.worst;
      optional.value = [...step4.attributes.optional];
      extra.value = [...step4.attributes.extra];
    }
  } catch (e: unknown) {
    console.warn(e);
  } // TODO(fpion): remove try..catch
});
</script>

<template>
  <div>
    <h3>{{ t("characters.steps.attributes") }}</h3>
    <form @submit="onSubmit">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("game.attribute.label") }}</th>
            <th scope="col">{{ t("characters.attributes.bases") }}</th>
            <th scope="col">{{ t("characters.lineage") }}</th>
            <th scope="col">{{ t("personalities.select.label") }}</th>
            <th scope="col">{{ t("characters.aspects.label") }}</th>
            <th scope="col">{{ t("game.attribute.score") }}</th>
            <th scope="col">{{ t("game.attribute.modifier") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="row in rows" :key="row.attribute">
            <td>{{ row.text }}</td>
            <td>
              <TarButton class="me-1" :disabled="row.base <= 6" icon="fas fa-minus" size="small" @click="decreaseBase(row)" />
              {{ row.base }}
              <TarButton class="ms-1" :disabled="row.base >= 11 || remainingPoints <= 0" icon="fas fa-plus" size="small" @click="increaseBase(row)" />
            </td>
            <td>{{ row.lineage }}</td>
            <td>{{ row.personality }}</td>
            <td>{{ row.aspects }}</td>
            <td>{{ row.score }}</td>
            <td>{{ row.modifier < 0 ? row.modifier : `+${row.modifier}` }}</td>
          </tr>
        </tbody>
      </table>
      <p v-if="remainingPoints > 0" class="text-danger">
        <font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("characters.attributes.spend", { n: remainingPoints }) }}
      </p>
      <h5>{{ t("characters.aspects.label") }}</h5>
      <h6>{{ t("aspects.attributes.mandatory") }}</h6>
      <p>TODO(fpion): mandatory attributes</p>
      <p class="text-danger"><font-awesome-icon icon="fas fa-triangle-exclamation" /> TODO(fpion): warning</p>
      <h6>{{ t("aspects.attributes.optional") }}</h6>
      <p>TODO(fpion): optional attributes</p>
      <p class="text-danger"><font-awesome-icon icon="fas fa-triangle-exclamation" /> TODO(fpion): warning</p>
      <template v-if="lineageAttributes.extra > 0">
        <h5>{{ t("characters.lineage") }}</h5>
        <p v-if="requiredExtra > 0" class="text-danger"><font-awesome-icon icon="fas fa-triangle-exclamation" /> requiredExtra: {{ requiredExtra }}</p>
        <p>TODO(fpion): extra attributes</p>
      </template>
      <TarButton class="me-1" icon="fas fa-arrow-left" :text="t('actions.back')" variant="secondary" @click="character.goBack()" />
      <TarButton class="ms-1" :disabled="!isCompleted" icon="fas fa-arrow-right" :text="t('actions.continue')" type="submit" />
    </form>
  </div>
</template>
