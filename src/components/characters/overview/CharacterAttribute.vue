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
    <TarModal centered :close="t('actions.close')" fade scrollable ref="modal" :title="t('game.attribute.format', { name })">
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
          <div>{{ formatSignedInteger(modifier.value, n) }}</div>
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
import type { Attribute } from "@/types/game";
import type { Character, CharacterModifier } from "@/types/characters";
import { ATTRIBUTE_CATEGORIES, camelCase } from "@/utils/game";
import { formatSignedInteger } from "@/utils/format";

const { n, t } = useI18n();
const { orderBy } = arrayUtils;

const props = defineProps<{
  attribute: Attribute;
  character: Character;
  name: string;
}>();

const modal = ref<InstanceType<typeof TarModal> | null>(null);

const category = computed<string>(() => t(`game.attribute.${ATTRIBUTE_CATEGORIES[props.attribute]}`));
const values = computed(() => props.character.attributes[camelCase(props.attribute)]);
const starting = computed<string>(() => formatSignedInteger(values.value.starting, n));
const progression = computed<string>(() => formatSignedInteger(values.value.progression, n));
const total = computed<string>(() => formatSignedInteger(values.value.total, n));

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
