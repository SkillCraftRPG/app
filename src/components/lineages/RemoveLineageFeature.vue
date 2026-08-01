<template>
  <div>
    <TarButton icon="fas fa-xmark" outline :text="t('actions.remove')" variant="danger" @click="open" />
    <TarModal centered :close="t('actions.close')" fade ref="modal" :title="t('lineages.features.remove.lead')">
      <div v-html="help"></div>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="isLoading"
          icon="fas fa-xmark"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('actions.remove')"
          variant="danger"
          @click="remove"
        />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { Lineage, LineageFeature } from "@/types/lineages";
import { deleteLineageFeature } from "@/api/lineages";

const { t } = useI18n();

const props = defineProps<{
  feature: LineageFeature;
  lineage: Lineage;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Lineage): void;
}>();

const isLoading = ref<boolean>(false);
const modal = ref<InstanceType<typeof TarModal> | null>(null);

const help = computed<string>(() => t("lineages.features.remove.help", { name: props.feature.name }));

function cancel(): void {
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}

async function remove(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const lineage: Lineage = await deleteLineageFeature(props.lineage.id, props.feature.id);
      modal.value?.hide();
      emit("updated", lineage);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>
