<script setup lang="ts">
import { TarModal } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import { TarButton } from "logitar-vue3-ui";
import type { BonusModel, CharacterModel } from "@/types/characters";
import { removeBonus } from "@/api/characters";
import { useToastStore } from "@/stores/toast";

const toasts = useToastStore();
const { t } = useI18n();

const props = defineProps<{
  bonus: BonusModel;
  character: CharacterModel;
}>();

const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

const id = computed<string>(() => `delete-bonus-${props.bonus.id}`);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: CharacterModel): void;
}>();

function hide(): void {
  modalRef.value?.hide();
}

function onCancel(): void {
  hide();
}

async function onRemove(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const character: CharacterModel = await removeBonus(props.character.id, props.bonus.id);
      toasts.success("characters.bonuses.removed");
      emit("updated", character);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
      hide();
    }
  }
}
</script>

<template>
  <span>
    <TarButton icon="fas fa-trash" :text="t('actions.remove')" variant="danger" data-bs-toggle="modal" :data-bs-target="`#${id}`" />
    <TarModal :close="t('actions.close')" :id="id" ref="modalRef" :title="t('characters.bonuses.remove.label')">
      <p>
        {{ t("characters.bonuses.remove.confirm") }}
        <br />
        <span class="text-danger">
          {{ bonus.value > 0 ? `+${bonus.value}` : bonus.value }}
          <template v-if="bonus.category === 'Attribute'">{{ t(`game.attribute.options.${bonus.target}`) }}</template>
          <template v-else-if="bonus.category === 'Miscellaneous'">{{ t(`characters.bonuses.miscellaneous.options.${bonus.target}`) }}</template>
          <template v-else-if="bonus.category === 'Skill'">{{ t(`game.skill.options.${bonus.target}`) }}</template>
          <template v-else-if="bonus.category === 'Speed'">{{ t(`game.speed.options.${bonus.target}`) }}</template>
          <template v-else-if="bonus.category === 'Statistic'">{{ t(`game.statistic.options.${bonus.target}`) }}</template>
          <template v-if="bonus.isTemporary"> ({{ t("characters.bonuses.temporary") }})</template>
          <template v-if="bonus.precision"> ({{ bonus.precision }})</template>
        </span>
      </p>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isLoading"
          icon="fas fa-trash"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('actions.remove')"
          variant="danger"
          @click="onRemove"
        />
      </template>
    </TarModal>
  </span>
</template>
