<template>
  <TarCard>
    <div class="card-text d-flex justify-content-between align-items-center gap-2">
      <div>
        <div class="fw-semibold">{{ modifier.name ?? target }}</div>
        <div class="text-body-secondary">
          <font-awesome-icon :icon="icon" aria-hidden="true" />&nbsp;{{ kind }}<template v-if="modifier.name"> ({{ target }})</template>
        </div>
      </div>
      <div class="fs-4">{{ value }}</div>
    </div>
    <div class="d-flex justify-content-end gap-2 mt-2">
      <TarButton icon="fas fa-edit" outline :text="t('actions.edit')" @click="$emit('edit')" />
      <TarButton icon="fas fa-xmark" outline :text="t('actions.remove')" variant="danger" @click="$emit('remove')" />
    </div>
  </TarCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
import TarCard from "@/components/tar/TarCard.vue";
import type { CharacterModifier } from "@/types/characters";
import { formatSignedInteger } from "@/utils/format";

const { n, t } = useI18n();

const props = defineProps<{
  modifier: CharacterModifier;
}>();

defineEmits<{
  (e: "edit"): void;
  (e: "remove"): void;
}>();

const icon = computed<string>(() => {
  switch (props.modifier.kind) {
    case "Attribute":
      return "fas fa-chart-simple";
    case "Skill":
      return "fas fa-kitchen-set";
    case "Speed":
      return "fas fa-person-running";
    case "Statistic":
      return "fas fa-magnifying-glass-chart";
  }
});
const kind = computed<string>(() => {
  switch (props.modifier.kind) {
    case "Attribute":
      return t("game.attribute.label");
    case "Skill":
      return t("game.skill.label");
    case "Speed":
      return t("game.speed.label");
    case "Statistic":
      return t("game.statistic.label");
  }
});
const target = computed<string>(() => {
  switch (props.modifier.kind) {
    case "Attribute":
      return t(`game.attribute.options.${props.modifier.target}`);
    case "Skill":
      return t(`game.skill.options.${props.modifier.target}`);
    case "Speed":
      return t(`game.speed.kind.options.${props.modifier.target}`);
    case "Statistic":
      return t(`game.statistic.options.${props.modifier.target}`);
  }
});
const value = computed<string>(() => formatSignedInteger(props.modifier.value, (value) => n(value, "integer")));
</script>
