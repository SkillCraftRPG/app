<template>
  <div>
    <TarButton :disabled="!character.points.attributes" icon="fas fa-arrow-turn-up" size="small" :text="increaseText" @click="open" />
    <TarModal centered :close="t('actions.close')" fade scrollable size="large" ref="modal" :title="t('characters.attributes.increase')">
      <div class="fw-semibold mb-3" :class="{ 'text-primary': remaining, 'text-body-secondary': !remaining }">
        {{ t("characters.attributes.remaining", { remaining }) }}
      </div>
      <div v-for="attribute in attributes" :key="attribute.key" class="row">
        <div class="row mb-1">
          <div class="col fs-5">{{ attribute.name }}</div>
          <div class="col text-center">
            <TarButton :disabled="!attribute.values.spent" icon="fas fa-minus" outline size="small" variant="secondary" @click="decrease(attribute.key)" />
          </div>
          <div class="col text-center">
            {{ formatSignedInteger(attribute.values.current, n) }}
          </div>
          <div class="col text-center" :class="{ 'text-primary': attribute.values.spent }">
            {{ formatSignedInteger(attribute.values.spent, n) }}
          </div>
          <div class="col text-center fw-semibold" :class="{ 'text-primary': attribute.values.spent }">
            {{ formatSignedInteger(attribute.values.increased, n) }}
          </div>
          <div class="col text-end">
            <TarButton :disabled="!attribute.increase" icon="fas fa-plus" outline size="small" @click="increase(attribute.key)" />
          </div>
        </div>
        <div class="row">
          <div class="col-md-6">
            <div v-for="statistic in attribute.statistics" :key="statistic.key" class="row">
              <div class="col">{{ statistic.name }}</div>
              <div class="col text-center">{{ n(statistic.values.current, "integer") }}</div>
              <div class="col text-center" :class="{ 'text-primary': statistic.values.delta }">{{ formatSignedInteger(statistic.values.delta, n) }}</div>
              <div class="col text-end fw-semibold" :class="{ 'text-primary': statistic.values.delta }">{{ n(statistic.values.increased, "integer") }}</div>
            </div>
          </div>
          <div class="col-md-6 mb-3">
            <div v-for="skill in attribute.skills" :key="skill.key" class="row">
              <div class="col">{{ skill.name }}</div>
              <div class="col text-center">{{ formatSignedInteger(skill.values.current, n) }}</div>
              <div class="col text-center" :class="{ 'text-primary': skill.values.delta }">{{ formatSignedInteger(skill.values.delta, n) }}</div>
              <div class="col text-end fw-semibold" :class="{ 'text-primary': skill.values.delta }">{{ formatSignedInteger(skill.values.increased, n) }}</div>
            </div>
          </div>
        </div>
      </div>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton :disabled="!spent" icon="fas fa-arrow-turn-up" :loading="isLoading" :status="t('loading')" :text="t('actions.increase')" @click="submit" />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { Character } from "@/types/characters";
import { calculateStatistic } from "@/utils/game";
import { formatSignedInteger } from "@/utils/format";

const MAXIMUM_SCORE: number = 5;
const { n, t } = useI18n();
const { orderBy } = arrayUtils;

const props = defineProps<{
  character: Character;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Character): void;
}>();

const dexterity = ref<number>(0);
const health = ref<number>(0);
const intellect = ref<number>(0);
const isLoading = ref<boolean>(false);
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const senses = ref<number>(0);
const vigor = ref<number>(0);

const increaseText = computed<string>(() => `${t("actions.increase")} (${formatSignedInteger(props.character.points.attributes, n)})`);
const spent = computed<number>(() => dexterity.value + health.value + intellect.value + senses.value + vigor.value);
const remaining = computed<number>(() => props.character.points.attributes - spent.value);

const attributes = computed(() => {
  const attributes = {
    dexterity: props.character.attributes.dexterity.total + dexterity.value,
    health: props.character.attributes.health.total + health.value,
    intellect: props.character.attributes.intellect.total + intellect.value,
    senses: props.character.attributes.senses.total + senses.value,
    vigor: props.character.attributes.vigor.total + vigor.value,
  };
  const dodge: number = calculateStatistic("Dodge", props.character.level, attributes) + props.character.statistics.dodge.modifiers;
  const initiative: number = calculateStatistic("Initiative", props.character.level, attributes) + props.character.statistics.initiative.modifiers;
  const learning: number = calculateStatistic("Learning", props.character.level, attributes) + props.character.statistics.learning.modifiers;
  const load: number = calculateStatistic("Load", props.character.level, attributes) + props.character.statistics.load.modifiers;
  const power: number = calculateStatistic("Power", props.character.level, attributes) + props.character.statistics.power.modifiers;
  const precision: number = calculateStatistic("Precision", props.character.level, attributes) + props.character.statistics.precision.modifiers;
  const stamina: number = calculateStatistic("Stamina", props.character.level, attributes) + props.character.statistics.stamina.modifiers;
  const stratagem: number = calculateStatistic("Stratagem", props.character.level, attributes) + props.character.statistics.stratagem.modifiers;
  const strength: number = calculateStatistic("Strength", props.character.level, attributes) + props.character.statistics.strength.modifiers;
  const vitality: number = calculateStatistic("Vitality", props.character.level, attributes) + props.character.statistics.vitality.modifiers;
  return orderBy(
    [
      {
        key: "Dexterity",
        name: t("game.attribute.options.Dexterity"),
        increase:
          remaining.value > 0 &&
          props.character.attributes.dexterity.starting + props.character.attributes.dexterity.progression + dexterity.value < MAXIMUM_SCORE,
        values: {
          current: props.character.attributes.dexterity.total,
          spent: dexterity.value,
          increased: props.character.attributes.dexterity.total + dexterity.value,
        },
        statistics: orderBy(
          [
            {
              key: "Dodge",
              name: t("game.statistic.options.Dodge"),
              values: {
                current: props.character.statistics.dodge.total,
                delta: dodge - props.character.statistics.dodge.total,
                increased: dodge,
              },
            },
            {
              key: "Precision",
              name: t("game.statistic.options.Precision"),
              values: {
                current: props.character.statistics.precision.total,
                delta: precision - props.character.statistics.precision.total,
                increased: precision,
              },
            },
          ],
          "name",
        ),
        skills: orderBy(
          [
            {
              key: "Acrobatics",
              name: t("game.skill.options.Acrobatics"),
              values: {
                current: props.character.skills.acrobatics.total,
                delta: dexterity.value,
                increased: props.character.skills.acrobatics.total + dexterity.value,
              },
            },
            {
              key: "Crafting",
              name: t("game.skill.options.Crafting"),
              values: {
                current: props.character.skills.crafting.total,
                delta: dexterity.value,
                increased: props.character.skills.crafting.total + dexterity.value,
              },
            },
            {
              key: "Orientation",
              name: t("game.skill.options.Orientation"),
              values: {
                current: props.character.skills.orientation.total,
                delta: dexterity.value,
                increased: props.character.skills.orientation.total + dexterity.value,
              },
            },
            {
              key: "Stealth",
              name: t("game.skill.options.Stealth"),
              values: {
                current: props.character.skills.stealth.total,
                delta: dexterity.value,
                increased: props.character.skills.stealth.total + dexterity.value,
              },
            },
            {
              key: "Thievery",
              name: t("game.skill.options.Thievery"),
              values: {
                current: props.character.skills.thievery.total,
                delta: dexterity.value,
                increased: props.character.skills.thievery.total + dexterity.value,
              },
            },
          ],
          "name",
        ),
      },
      {
        key: "Health",
        name: t("game.attribute.options.Health"),
        increase:
          remaining.value > 0 && props.character.attributes.health.starting + props.character.attributes.health.progression + health.value < MAXIMUM_SCORE,
        values: {
          current: props.character.attributes.health.total,
          spent: health.value,
          increased: props.character.attributes.health.total + health.value,
        },
        statistics: orderBy(
          [
            {
              key: "Stamina",
              name: t("game.statistic.options.Stamina"),
              values: {
                current: props.character.statistics.stamina.total,
                delta: stamina - props.character.statistics.stamina.total,
                increased: stamina,
              },
            },
            {
              key: "Vitality",
              name: t("game.statistic.options.Vitality"),
              values: {
                current: props.character.statistics.vitality.total,
                delta: vitality - props.character.statistics.vitality.total,
                increased: vitality,
              },
            },
          ],
          "name",
        ),
        skills: orderBy(
          [
            {
              key: "Discipline",
              name: t("game.skill.options.Discipline"),
              values: {
                current: props.character.skills.discipline.total,
                delta: health.value,
                increased: props.character.skills.discipline.total + health.value,
              },
            },
            {
              key: "Resistance",
              name: t("game.skill.options.Resistance"),
              values: {
                current: props.character.skills.resistance.total,
                delta: health.value,
                increased: props.character.skills.resistance.total + health.value,
              },
            },
          ],
          "name",
        ),
      },
      {
        key: "Intellect",
        name: t("game.attribute.options.Intellect"),
        increase:
          remaining.value > 0 &&
          props.character.attributes.intellect.starting + props.character.attributes.intellect.progression + intellect.value < MAXIMUM_SCORE,
        values: {
          current: props.character.attributes.intellect.total,
          spent: intellect.value,
          increased: props.character.attributes.intellect.total + intellect.value,
        },
        statistics: orderBy(
          [
            {
              key: "Learning",
              name: t("game.statistic.options.Learning"),
              values: {
                current: props.character.statistics.learning.total,
                delta: learning - props.character.statistics.learning.total,
                increased: learning,
              },
            },
            {
              key: "Stratagem",
              name: t("game.statistic.options.Stratagem"),
              values: {
                current: props.character.statistics.stratagem.total,
                delta: stratagem - props.character.statistics.stratagem.total,
                increased: stratagem,
              },
            },
          ],
          "name",
        ),
        skills: orderBy(
          [
            {
              key: "Investigation",
              name: t("game.skill.options.Investigation"),
              values: {
                current: props.character.skills.investigation.total,
                delta: intellect.value,
                increased: props.character.skills.investigation.total + intellect.value,
              },
            },
            {
              key: "Knowledge",
              name: t("game.skill.options.Knowledge"),
              values: {
                current: props.character.skills.knowledge.total,
                delta: intellect.value,
                increased: props.character.skills.knowledge.total + intellect.value,
              },
            },
            {
              key: "Linguistics",
              name: t("game.skill.options.Linguistics"),
              values: {
                current: props.character.skills.linguistics.total,
                delta: intellect.value,
                increased: props.character.skills.linguistics.total + intellect.value,
              },
            },
            {
              key: "Medicine",
              name: t("game.skill.options.Medicine"),
              values: {
                current: props.character.skills.medicine.total,
                delta: intellect.value,
                increased: props.character.skills.medicine.total + intellect.value,
              },
            },
          ],
          "name",
        ),
      },
      {
        key: "Senses",
        name: t("game.attribute.options.Senses"),
        increase:
          remaining.value > 0 && props.character.attributes.senses.starting + props.character.attributes.senses.progression + senses.value < MAXIMUM_SCORE,
        values: {
          current: props.character.attributes.senses.total,
          spent: senses.value,
          increased: props.character.attributes.senses.total + senses.value,
        },
        statistics: orderBy(
          [
            {
              key: "Initiative",
              name: t("game.statistic.options.Initiative"),
              values: {
                current: props.character.statistics.initiative.total,
                delta: initiative - props.character.statistics.initiative.total,
                increased: initiative,
              },
            },
            {
              key: "Power",
              name: t("game.statistic.options.Power"),
              values: {
                current: props.character.statistics.power.total,
                delta: power - props.character.statistics.power.total,
                increased: power,
              },
            },
          ],
          "name",
        ),
        skills: orderBy(
          [
            {
              key: "Insight",
              name: t("game.skill.options.Insight"),
              values: {
                current: props.character.skills.insight.total,
                delta: senses.value,
                increased: props.character.skills.insight.total + senses.value,
              },
            },
            {
              key: "Occultism",
              name: t("game.skill.options.Occultism"),
              values: {
                current: props.character.skills.occultism.total,
                delta: senses.value,
                increased: props.character.skills.occultism.total + senses.value,
              },
            },
            {
              key: "Perception",
              name: t("game.skill.options.Perception"),
              values: {
                current: props.character.skills.perception.total,
                delta: senses.value,
                increased: props.character.skills.perception.total + senses.value,
              },
            },
            {
              key: "Survival",
              name: t("game.skill.options.Survival"),
              values: {
                current: props.character.skills.survival.total,
                delta: senses.value,
                increased: props.character.skills.survival.total + senses.value,
              },
            },
          ],
          "name",
        ),
      },
      {
        key: "Vigor",
        name: t("game.attribute.options.Vigor"),
        increase: remaining.value > 0 && props.character.attributes.vigor.starting + props.character.attributes.vigor.progression + vigor.value < MAXIMUM_SCORE,
        values: {
          current: props.character.attributes.vigor.total,
          spent: vigor.value,
          increased: props.character.attributes.vigor.total + vigor.value,
        },
        statistics: orderBy(
          [
            {
              key: "Load",
              name: t("game.statistic.options.Load"),
              values: {
                current: props.character.statistics.load.total,
                delta: load - props.character.statistics.load.total,
                increased: load,
              },
            },
            {
              key: "Strength",
              name: t("game.statistic.options.Strength"),
              values: {
                current: props.character.statistics.strength.total,
                delta: strength - props.character.statistics.strength.total,
                increased: strength,
              },
            },
          ],
          "name",
        ),
        skills: orderBy(
          [
            {
              key: "Athletics",
              name: t("game.skill.options.Athletics"),
              values: {
                current: props.character.skills.athletics.total,
                delta: vigor.value,
                increased: props.character.skills.athletics.total + vigor.value,
              },
            },
            {
              key: "Melee",
              name: t("game.skill.options.Melee"),
              values: {
                current: props.character.skills.melee.total,
                delta: vigor.value,
                increased: props.character.skills.melee.total + vigor.value,
              },
            },
          ],
          "name",
        ),
      },
    ],
    "name",
  );
});

function decrease(attribute: string): void {
  switch (attribute) {
    case "Dexterity":
      dexterity.value--;
      break;
    case "Health":
      health.value--;
      break;
    case "Intellect":
      intellect.value--;
      break;
    case "Senses":
      senses.value--;
      break;
    case "Vigor":
      vigor.value--;
      break;
  }
}
function increase(attribute: string): void {
  switch (attribute) {
    case "Dexterity":
      dexterity.value++;
      break;
    case "Health":
      health.value++;
      break;
    case "Intellect":
      intellect.value++;
      break;
    case "Senses":
      senses.value++;
      break;
    case "Vigor":
      vigor.value++;
      break;
  }
}

function reset(): void {
  dexterity.value = 0;
  health.value = 0;
  intellect.value = 0;
  senses.value = 0;
  vigor.value = 0;
}

async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      console.log("Submitting!"); // TODO(fpion): implement
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

function cancel(): void {
  reset();
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}
</script>
