<template>
  <div>
    <TarCard class="clickable" @click="open">
      <div class="card-text d-flex justify-content-between align-items-center gap-2">
        <div>
          <div class="fw-semibold">{{ name }}</div>
          <div class="small text-body-secondary">{{ attribute }}</div>
        </div>
        <div class="fs-4">{{ total }}</div>
      </div>
    </TarCard>
    <TarModal centered :close="t('actions.close')" fade scrollable ref="modal" :title="t('game.skill.format', { name })">
      <div class="card-text">
        <div class="d-flex justify-content-between gap-2">
          <div>{{ t("game.skill.rank") }}</div>
          <div>{{ rank }}</div>
        </div>
        <div v-for="talent in talents" :key="talent.id" class="d-flex justify-content-between gap-2">
          <div>{{ talent.name }}</div>
          <div>{{ formatSignedInteger(1, (value) => n(value, "integer")) }}</div>
        </div>
        <div class="d-flex justify-content-between gap-2">
          <div>{{ attribute }}</div>
          <div>{{ attributeScore }}</div>
        </div>
        <div v-for="modifier in modifiers" :key="modifier.id" class="d-flex justify-content-between gap-2">
          <div>{{ modifier.label }}</div>
          <div>{{ formatSignedInteger(modifier.value, (value) => n(value, "integer")) }}</div>
        </div>
        <hr />
        <div class="d-flex justify-content-between gap-2 fw-semibold">
          <div>{{ t("total") }}</div>
          <div>{{ total }}</div>
        </div>
      </div>
      <template #footer>
        <TarButton icon="fas fa-xmark" :text="t('actions.close')" variant="secondary" @click="close" />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
import TarCard from "@/components/tar/TarCard.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { Character, CharacterModifier } from "@/types/characters";
import type { Skill } from "@/types/game";
import { SKILL_ATTRIBUTES, camelCase } from "@/utils/game";
import { formatCharacterTalent } from "@/utils/talent";
import { formatSignedInteger } from "@/utils/format";

const { n, t } = useI18n();
const { orderBy } = arrayUtils;

const props = defineProps<{
  character: Character;
  name: string;
  skill: Skill;
}>();

const modal = ref<InstanceType<typeof TarModal> | null>(null);

const attribute = computed<string>(() => {
  const key = SKILL_ATTRIBUTES[props.skill];
  return key ? t(`game.attribute.options.${key}`) : t("characters.social");
});
const values = computed(() => props.character.skills[camelCase(props.skill)]);
const rank = computed<string>(() => formatSignedInteger(values.value.rank, (value) => n(value, "integer")));
const attributeScore = computed<string>(() => formatSignedInteger(values.value.attribute, (value) => n(value, "integer")));
const total = computed<string>(() => formatSignedInteger(values.value.total, (value) => n(value, "integer")));

type SkillTalent = {
  id: string;
  name: string;
  tier: number;
};
const talents = computed<SkillTalent[]>(() =>
  orderBy(
    orderBy(
      props.character.talents
        .filter((acquisition) => acquisition.talent.skill === props.skill)
        .map((acquisition) => ({
          id: acquisition.id,
          name: formatCharacterTalent(acquisition),
          tier: acquisition.talent.tier,
        })),
      "name",
    ),
    "tier",
  ),
);

type LabelledCharacterModifier = CharacterModifier & {
  label: string;
};
const modifiers = computed<LabelledCharacterModifier[]>(() =>
  orderBy(
    props.character.modifiers
      .filter((modifier) => modifier.kind === "Skill" && modifier.target === props.skill)
      .map((modifier) => ({ ...modifier, label: modifier.name ?? t("characters.modifiers.label") })),
    "label",
  ),
);

function close(): void {
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}
</script>
