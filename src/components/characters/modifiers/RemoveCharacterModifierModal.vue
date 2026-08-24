<template>
  <TarModal centered :close="t('actions.close')" fade scrollable ref="modal" :title="t('characters.modifiers.remove.lead')">
    <p class="mb-0" v-html="help"></p>
    <template #footer>
      <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
      <TarButton
        :disabled="isLoading"
        icon="fas fa-xmark"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('actions.remove')"
        variant="danger"
        @click="confirm"
      />
    </template>
  </TarModal>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { Character, CharacterModifier } from "@/types/characters";
import { formatSignedInteger } from "@/utils/format";
import { removeCharacterModifier } from "@/api/characters";
import { translateKind, translateTarget } from "@/utils/modifier";

const { n, t } = useI18n();

const props = defineProps<{
  character: Character;
  modifier: CharacterModifier;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Character): void;
}>();

const isLoading = ref<boolean>(false);
const modal = ref<InstanceType<typeof TarModal> | null>(null);

const name = computed<string>(() => {
  if (props.modifier.name) {
    return props.modifier.name;
  }
  const values: string[] = [];
  values.push(translateKind(props.modifier, t));
  values.push(`(${translateTarget(props.modifier, t)})`);
  values.push(formatSignedInteger(props.modifier.value, n));
  return values.join(" ");
});
const help = computed<string>(() => t("characters.modifiers.remove.help", { name: name.value }));

function cancel(): void {
  modal.value?.hide();
}

async function confirm(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const character: Character = await removeCharacterModifier(props.character.id, props.modifier.id);
      emit("updated", character);
      modal.value?.hide();
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

function open(): void {
  modal.value?.show();
}
defineExpose({ open });
</script>
