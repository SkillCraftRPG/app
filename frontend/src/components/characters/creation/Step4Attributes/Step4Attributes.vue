<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import MandatoryAttributeCard from "./MandatoryAttributeCard.vue";
import OptionalAttributeCard from "./OptionalAttributeCard.vue";
import type { Attribute } from "@/types/game";
import type { AttributeBonusesModel, LineageModel } from "@/types/lineages";
import type { MandatoryAttribute } from "@/types/aspects";
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
  nature: number;
  aspects: number;
  score: number;
  modifier: number;
};
type OptionalAttribute = {
  attribute: Attribute;
  text: string;
  selected: boolean;
};

const agility = ref<number>(8);
const coordination = ref<number>(8);
const extra = ref<Attribute[]>([]);
const intellect = ref<number>(8);
const mandatory = ref<MandatoryAttribute[]>([]);
const optional = ref<OptionalAttribute[]>([]);
const presence = ref<number>(8);
const sensitivity = ref<number>(8);
const spirit = ref<number>(8);
const vigor = ref<number>(8);

function calculateAspectBonus(attribute: Attribute): number {
  let bonus: number = 0;
  mandatory.value
    .filter((mandatory) => mandatory.attribute === attribute)
    .forEach(({ selected }) => {
      switch (selected) {
        case "best":
          bonus += 3;
          break;
        case "worst":
          bonus += 1;
          break;
        default:
          bonus += 2;
          break;
      }
    });
  bonus += optional.value.filter((optional) => optional.attribute === attribute && optional.selected).length;
  return bonus;
}

const best = computed<Attribute | undefined>(() => {
  const best: Attribute[] = mandatory.value.filter(({ selected }) => selected === "best").map(({ attribute }) => attribute);
  return best.length === 1 ? best[0] : undefined;
});
const isCompleted = computed<boolean>(
  () =>
    remainingPoints.value === 0 &&
    Boolean(best.value) &&
    Boolean(worst.value) &&
    best.value !== worst.value &&
    requiredOptional.value === 0 &&
    requiredExtra.value === 0,
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
const requiredOptional = computed<number>(() => 2 - optional.value.filter(({ selected }) => selected).length);
const rows = computed<AttributeRow[]>(() => {
  const step2: Step2 | undefined = character.creation.step2;
  const rows: AttributeRow[] = [
    {
      attribute: "Agility",
      text: t("game.attribute.options.Agility"),
      base: agility.value,
      lineage: lineageAttributes.value.agility + (extra.value.includes("Agility") ? 1 : 0),
      nature: step2?.nature.attribute === "Agility" ? 1 : 0,
      aspects: calculateAspectBonus("Agility"),
      score: 0,
      modifier: 0,
    },
    {
      attribute: "Coordination",
      text: t("game.attribute.options.Coordination"),
      base: coordination.value,
      lineage: lineageAttributes.value.coordination + (extra.value.includes("Coordination") ? 1 : 0),
      nature: step2?.nature.attribute === "Coordination" ? 1 : 0,
      aspects: calculateAspectBonus("Coordination"),
      score: 0,
      modifier: 0,
    },
    {
      attribute: "Intellect",
      text: t("game.attribute.options.Intellect"),
      base: intellect.value,
      lineage: lineageAttributes.value.intellect + (extra.value.includes("Intellect") ? 1 : 0),
      nature: step2?.nature.attribute === "Intellect" ? 1 : 0,
      aspects: calculateAspectBonus("Intellect"),
      score: 0,
      modifier: 0,
    },
    {
      attribute: "Presence",
      text: t("game.attribute.options.Presence"),
      base: presence.value,
      lineage: lineageAttributes.value.presence + (extra.value.includes("Presence") ? 1 : 0),
      nature: step2?.nature.attribute === "Presence" ? 1 : 0,
      aspects: calculateAspectBonus("Presence"),
      score: 0,
      modifier: 0,
    },
    {
      attribute: "Sensitivity",
      text: t("game.attribute.options.Sensitivity"),
      base: sensitivity.value,
      lineage: lineageAttributes.value.sensitivity + (extra.value.includes("Sensitivity") ? 1 : 0),
      nature: step2?.nature.attribute === "Sensitivity" ? 1 : 0,
      aspects: calculateAspectBonus("Sensitivity"),
      score: 0,
      modifier: 0,
    },
    {
      attribute: "Spirit",
      text: t("game.attribute.options.Spirit"),
      base: spirit.value,
      lineage: lineageAttributes.value.spirit + (extra.value.includes("Spirit") ? 1 : 0),
      nature: step2?.nature.attribute === "Spirit" ? 1 : 0,
      aspects: calculateAspectBonus("Spirit"),
      score: 0,
      modifier: 0,
    },
    {
      attribute: "Vigor",
      text: t("game.attribute.options.Vigor"),
      base: vigor.value,
      lineage: lineageAttributes.value.vigor + (extra.value.includes("Vigor") ? 1 : 0),
      nature: step2?.nature.attribute === "Vigor" ? 1 : 0,
      aspects: calculateAspectBonus("Vigor"),
      score: 0,
      modifier: 0,
    },
  ];
  rows.forEach((row) => {
    row.score = row.base + row.lineage + row.nature + row.aspects;
    row.modifier = calculateModifier(row.score);
  });
  return orderBy(rows, "text");
});
const worst = computed<Attribute | undefined>(() => {
  const worst: Attribute[] = mandatory.value.filter(({ selected }) => selected === "worst").map(({ attribute }) => attribute);
  return worst.length === 1 ? worst[0] : undefined;
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

function setBest(index: number): void {
  for (let i = 0; i < mandatory.value.length; i++) {
    let attribute: MandatoryAttribute = mandatory.value[i];
    if (i === index) {
      if (attribute.selected !== "best") {
        attribute = { ...attribute, selected: "best" };
        mandatory.value.splice(i, 1, attribute);
      }
    } else if (attribute.selected === "best") {
      attribute = { ...attribute, selected: "mandatory" };
      mandatory.value.splice(i, 1, attribute);
    }
  }
}
function setMandatory(index: number): void {
  let attribute: MandatoryAttribute | undefined = mandatory.value[index];
  if (attribute?.selected !== "mandatory") {
    attribute = { ...attribute, selected: "mandatory" };
    mandatory.value.splice(index, 1, attribute);
  }
}
function setWorst(index: number): void {
  for (let i = 0; i < mandatory.value.length; i++) {
    let attribute: MandatoryAttribute = mandatory.value[i];
    if (i === index) {
      if (attribute.selected !== "worst") {
        attribute = { ...attribute, selected: "worst" };
        mandatory.value.splice(i, 1, attribute);
      }
    } else if (attribute.selected === "worst") {
      attribute = { ...attribute, selected: "mandatory" };
      mandatory.value.splice(i, 1, attribute);
    }
  }
}

function toggleExtra(attribute: Attribute): void {
  const index: number = extra.value.findIndex((a) => a === attribute);
  if (index < 0) {
    extra.value.push(attribute);
  } else {
    extra.value.splice(index, 1);
  }
}

function toggleOptional(index: number): void {
  let attribute: OptionalAttribute | undefined = optional.value[index];
  if (attribute) {
    attribute = { ...attribute, selected: !attribute.selected };
    optional.value.splice(index, 1, attribute);
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
        optional: optional.value.filter(({ selected }) => selected).map(({ attribute }) => attribute),
        extra: extra.value,
      },
    };
    character.setStep4(payload);
    character.next();
  }
});

onMounted(() => {
  const step3: Step3 | undefined = character.creation.step3;
  if (step3) {
    step3.aspects.forEach(({ attributes }) => {
      if (attributes.mandatory1) {
        mandatory.value.push({ attribute: attributes.mandatory1, text: t(`game.attribute.options.${attributes.mandatory1}`), selected: "mandatory" });
      }
      if (attributes.mandatory2) {
        mandatory.value.push({ attribute: attributes.mandatory2, text: t(`game.attribute.options.${attributes.mandatory2}`), selected: "mandatory" });
      }
      if (attributes.optional1) {
        optional.value.push({ attribute: attributes.optional1, text: t(`game.attribute.options.${attributes.optional1}`), selected: false });
      }
      if (attributes.optional2) {
        optional.value.push({ attribute: attributes.optional2, text: t(`game.attribute.options.${attributes.optional2}`), selected: false });
      }
    });
    mandatory.value = orderBy(mandatory.value, "text");
    optional.value = orderBy(optional.value, "text");
  }
  const step4: Step4 | undefined = character.creation.step4;
  if (step4) {
    agility.value = step4.attributes.agility;
    coordination.value = step4.attributes.coordination;
    intellect.value = step4.attributes.intellect;
    presence.value = step4.attributes.presence;
    sensitivity.value = step4.attributes.sensitivity;
    spirit.value = step4.attributes.spirit;
    vigor.value = step4.attributes.vigor;

    let index: number = mandatory.value.findIndex(({ attribute }) => attribute === step4.attributes.best);
    if (index >= 0) {
      const attribute: MandatoryAttribute = { ...mandatory.value[index], selected: "best" };
      mandatory.value.splice(index, 1, attribute);
    }
    index = mandatory.value.findIndex(({ attribute }) => attribute === step4.attributes.worst);
    if (index >= 0) {
      const attribute: MandatoryAttribute = { ...mandatory.value[index], selected: "worst" };
      mandatory.value.splice(index, 1, attribute);
    }

    step4.attributes.optional.forEach((attribute) => {
      const index: number = optional.value.findIndex((optional) => optional.attribute === attribute && !optional.selected);
      if (index >= 0) {
        const attribute: OptionalAttribute = { ...optional.value[index], selected: true };
        optional.value.splice(index, 1, attribute);
      }
    });

    extra.value = [...step4.attributes.extra];
  }
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
            <th scope="col">{{ t("natures.select.label") }}</th>
            <th scope="col">{{ t("aspects.list") }}</th>
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
            <td>{{ row.nature }}</td>
            <td>{{ row.aspects }}</td>
            <td>{{ row.score }}</td>
            <td>{{ row.modifier < 0 ? row.modifier : `+${row.modifier}` }}</td>
          </tr>
        </tbody>
      </table>
      <p v-if="remainingPoints > 0" class="text-danger">
        <font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("characters.attributes.spend", { n: remainingPoints }) }}
      </p>
      <h5>{{ t("aspects.list") }}</h5>
      <h6>{{ t("aspects.attributes.mandatory") }}</h6>
      <div class="mb-3 row">
        <div v-for="(mandatory, index) in mandatory" :key="index" class="col">
          <MandatoryAttributeCard v-bind="mandatory" @best="setBest(index)" @mandatory="setMandatory(index)" @worst="setWorst(index)" />
        </div>
      </div>
      <p v-if="!best" class="text-danger"><font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("characters.attributes.best.select") }}</p>
      <p v-else-if="!worst" class="text-danger"><font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("characters.attributes.worst.select") }}</p>
      <p v-if="best && best === worst" class="text-danger">
        <font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("characters.attributes.bestCannotBeWorst") }}
      </p>
      <h6>{{ t("aspects.attributes.optional") }}</h6>
      <div class="align-items-stretch mb-3 row">
        <div v-for="(optional, index) in optional" :key="index" class="col">
          <OptionalAttributeCard :attribute="optional.attribute" class="h-100" :selected="optional.selected" @click="toggleOptional(index)" />
        </div>
      </div>
      <p v-if="requiredOptional < 0" class="text-danger">
        <font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("characters.attributes.optional.less", { n: -requiredOptional }) }}
      </p>
      <p v-if="requiredOptional > 0" class="text-danger">
        <font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("characters.attributes.optional.more", { n: requiredOptional }) }}
      </p>
      <template v-if="lineageAttributes.extra > 0">
        <h5>{{ t("characters.lineage") }}</h5>
        <div class="mb-3 row">
          <div v-for="row in rows" :key="row.attribute" class="col">
            <OptionalAttributeCard :attribute="row.attribute" :selected="extra.includes(row.attribute)" @click="toggleExtra(row.attribute)" />
          </div>
        </div>
        <p v-if="requiredExtra < 0" class="text-danger">
          <font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("characters.attributes.extra.less", { n: -requiredExtra }) }}
        </p>
        <p v-if="requiredExtra > 0" class="text-danger">
          <font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("characters.attributes.extra.more", { n: requiredExtra }) }}
        </p>
      </template>
      <TarButton class="me-1" icon="fas fa-arrow-left" :text="t('actions.back')" variant="secondary" @click="character.back()" />
      <TarButton class="ms-1" :disabled="!isCompleted" icon="fas fa-arrow-right" :text="t('actions.continue')" type="submit" />
    </form>
  </div>
</template>
