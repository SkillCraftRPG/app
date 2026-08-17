<template>
  <div>
    <TarButton icon="fas fa-arrow-right-from-bracket" :text="t('account.signOut.all.submit')" variant="danger" @click="open" />
    <TarModal centered :close="t('actions.close')" fade ref="modal" :title="t('account.signOut.all.lead')">
      <p class="mb-0">{{ t("account.signOut.all.help") }}</p>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="isLoading"
          icon="fas fa-arrow-right-from-bracket"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('account.signOut.all.submit')"
          variant="danger"
          @click="signOutEverywhere"
        />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import { signOut } from "@/api/account";
import { useAccountStore } from "@/stores/account";
import { useWorldStore } from "@/stores/world";

const account = useAccountStore();
const router = useRouter();
const world = useWorldStore();
const { t } = useI18n();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

const isLoading = ref<boolean>(false);
const modal = ref<InstanceType<typeof TarModal> | null>(null);

function cancel(): void {
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}

async function signOutEverywhere(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      await signOut(true);
      account.signOut("closed");
      world.exit();
      modal.value?.hide();
      router.push({ name: "SignIn" });
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>
