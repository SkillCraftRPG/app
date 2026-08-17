<template>
  <div>
    <TarCard class="clickable" @click="open">
      <div class="card-text d-flex justify-content-between align-items-center gap-2">
        <div>
          <div class="fw-semibold">{{ name }}</div>
          <div class="small text-body-secondary">{{ category }}</div>
        </div>
        <div class="fs-4">{{ total }}</div>
      </div>
    </TarCard>
    <TarModal centered :close="t('actions.close')" fade ref="modal" :title="t('game.attribute.format', { name })">
      <div class="card-text">
        <div class="d-flex justify-content-between gap-2">
          <div>{{ t("game.attribute.starting") }}</div>
          <div>{{ starting }}</div>
        </div>
        <div class="d-flex justify-content-between gap-2">
          <div>{{ t("game.attribute.progression") }}</div>
          <div>{{ progression }}</div>
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
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="close" />
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
import type { Attribute } from "@/types/game";
import type { Character, CharacterAttribute, CharacterModifier } from "@/types/characters";
import { formatSignedInteger } from "@/utils/format";

const { n, t } = useI18n();
const { orderBy } = arrayUtils;

const props = defineProps<{
  attribute: Attribute;
  character: Character;
  name: string;
}>();

const modal = ref<InstanceType<typeof TarModal> | null>(null);

const category = computed<string>(() => {
  switch (props.attribute) {
    case "Dexterity":
    case "Vigor":
      return t("game.attribute.physical");
    case "Intellect":
    case "Senses":
      return t("game.attribute.mental");
    default:
      return t("game.attribute.universal");
  }
});

const values = computed<CharacterAttribute>(() => {
  switch (props.attribute) {
    case "Dexterity":
      return props.character.attributes.dexterity;
    case "Health":
      return props.character.attributes.health;
    case "Intellect":
      return props.character.attributes.intellect;
    case "Senses":
      return props.character.attributes.senses;
    case "Vigor":
      return props.character.attributes.vigor;
  }
});
const starting = computed<string>(() => formatSignedInteger(values.value.starting, (value) => n(value, "integer")));
const progression = computed<string>(() => formatSignedInteger(values.value.progression, (value) => n(value, "integer")));
const total = computed<string>(() => formatSignedInteger(values.value.total, (value) => n(value, "integer")));

type LabelledCharacterModifier = CharacterModifier & {
  label: string;
};
const modifiers = computed<LabelledCharacterModifier[]>(() =>
  orderBy(
    props.character.modifiers
      .filter((modifier) => modifier.kind === "Attribute" && modifier.target === props.attribute)
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
