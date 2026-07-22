<template>
  <div>
    <TarProgress class="mb-3" :value="progress" />
    <p v-if="step === Step.Identity">{{ t("account.profile.completion.identity.help") }}</p>
    <form @submit.prevent="submit">
      <div v-if="step === Step.Identity">
        <div class="row mb-3">
          <div class="col-md-6">
            <FirstNameInput required v-model="firstName" />
          </div>
          <div class="col-md-6">
            <LastNameInput required v-model="lastName" />
          </div>
        </div>
      </div>
      <div class="d-flex justify-content-between">
        <div class="d-flex gap-2">
          <TarButton icon="fas fa-xmark" outline :text="t('actions.abort')" type="button" variant="danger" />
          <TarButton
            v-if="step !== Step.Identity"
            icon="fas fa-arrow-left"
            outline
            :text="t('actions.previous')"
            type="button"
            variant="secondary"
            @click="previous"
          />
        </div>
        <div class="d-flex gap-2">
          <TarButton icon="fas fa-check" :outline="step !== Step.Role" :text="t('actions.complete')" type="submit" />
          <TarButton v-if="step !== Step.Role" icon="fas fa-arrow-right" :text="t('actions.next')" type="submit" />
        </div>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import FirstNameInput from "./FirstNameInput.vue";
import LastNameInput from "./LastNameInput.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarProgress from "@/components/tar/TarProgress.vue";

const { t } = useI18n();

enum Step {
  Identity = 0,
  Security = 1,
  Personal = 2,
  Role = 3,
}

defineProps<{
  token: string;
}>();

const firstName = ref<string>("");
const lastName = ref<string>("");
const step = ref<Step>(Step.Identity);

const progress = computed<number>(() => Math.floor((step.value * 100) / 3));

function previous(): void {
  if (step.value !== Step.Identity) {
    step.value--;
  }
}

async function submit(): Promise<void> {
  if (step.value !== Step.Role) {
    step.value++;
  }
  // TODO(fpion): implement
}
</script>
