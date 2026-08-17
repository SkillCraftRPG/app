<template>
  <div>
    <TarCard class="clickable" @click="open">
      <div class="card-text d-flex justify-content-between align-items-center gap-2">
        <div>
          <div class="fw-semibold">{{ name }}</div>
        </div>
        <div class="fs-4">{{ total }}</div>
      </div>
    </TarCard>
    <TarModal centered :close="t('actions.close')" fade ref="modal" :title="t('game.speed.format', { name })">
      <div class="card-text">
        <div class="d-flex justify-content-between gap-2">
          <div>{{ t("game.speed.lineage") }}</div>
          <div>{{ lineage }}</div>
        </div>
        <div class="d-flex justify-content-between gap-2">
          <div>{{ t("game.speed.encumbrance") }}</div>
          <div>{{ encumbrance }}</div>
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
import type { SpeedKind } from "@/types/game";
import { camelCase } from "@/utils/game";
import { formatSignedInteger } from "@/utils/format";

const { n, t } = useI18n();
const { orderBy } = arrayUtils;

const props = defineProps<{
  character: Character;
  kind: SpeedKind;
  name: string;
}>();

const modal = ref<InstanceType<typeof TarModal> | null>(null);

const values = computed(() => props.character.speeds[camelCase(props.kind)]);
const lineage = computed<string>(() => n(values.value.lineage, "integer"));
const encumbrance = computed<string>(() => formatSignedInteger(values.value.encumbrance, (value) => n(value, "integer")));
const total = computed<string>(() => n(Math.max(0, values.value.total), "integer"));

type LabelledCharacterModifier = CharacterModifier & {
  label: string;
};
const modifiers = computed<LabelledCharacterModifier[]>(() =>
  orderBy(
    props.character.modifiers
      .filter((modifier) => modifier.kind === "Speed" && modifier.target === props.kind)
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
