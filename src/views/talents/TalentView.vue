<template>
  <main class="container page">
    <div v-if="talent">
      <div class="d-flex flex-wrap align-items-center gap-2">
        <h1>{{ title }}</h1>
        <TalentTierDisplay class="fs-6" :tier="talent.tier" />
      </div>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("talents.created.lead") }}</strong> {{ t("talents.created.help", { name: title }) }}
      </TarAlert>
      <StatusDetail class="mb-3" :subject="talent" />
      <form class="border-top border-secondary-subtle pt-4" @submit.prevent="handleSubmit(submit)">
        <NameField class="mb-3" required v-model="name" />
        <TarCheckbox
          class="mb-3"
          described-by="allow-multiple-purchases-help"
          :disabled="Boolean(skill)"
          id="allow-multiple-purchases"
          :label="t('talents.allowMultiplePurchases.label')"
          switch
          v-model="allowMultiplePurchases"
        >
          <template #after>
            <div id="allow-multiple-purchases-help" class="form-text">{{ t("talents.allowMultiplePurchases.help") }}</div>
          </template>
        </TarCheckbox>
        <div class="row">
          <div class="col-md-6">
            <SkillField class="mb-3" :disabled="allowMultiplePurchases" v-model="skill" />
          </div>
          <div class="col-md-6">
            <TalentField
              class="mb-3"
              :exclude="[talent.id]"
              id="required-talent"
              label="talents.required"
              :model-value="requiredTalent?.id ?? ''"
              :tiers="[0, 1, 2, 3].filter((tier) => tier <= talent!.tier)"
              @error="handleError"
              @selected="requiredTalent = $event"
            />
          </div>
        </div>
        <SummaryField class="mb-3" v-model="summary" />
        <ContentField class="mb-3" v-model="content" />
        <div class="d-flex justify-content-end mb-3">
          <TarButton
            :disabled="!hasChanges || isLoading"
            icon="fas fa-floppy-disk"
            :loading="isLoading"
            size="large"
            :status="t('loading')"
            :text="t('actions.save')"
            type="submit"
          />
        </div>
      </form>
    </div>
    <LoadingSpinner v-else />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import ContentField from "@/components/shared/ContentField.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import NameField from "@/components/shared/NameField.vue";
import SkillField from "@/components/game/SkillField.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TalentField from "@/components/talents/TalentField.vue";
import TalentTierDisplay from "@/components/talents/TalentTierDisplay.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarCheckbox from "@/components/tar/TarCheckbox.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { CreateOrReplaceTalentPayload, Talent } from "@/types/talents";
import type { Skill } from "@/types/game";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readTalent, replaceTalent } from "@/api/talents";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";
import { useForm } from "@/forms";
import { useToastStore } from "@/stores/toast";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const allowMultiplePurchases = ref<boolean>(false);
const content = ref<string>("");
const isCreated = ref<boolean>(events.shift() === "created");
const isLoading = ref<boolean>(false);
const name = ref<string>("");
const requiredTalent = ref<Talent>();
const skill = ref<string>("");
const summary = ref<string>("");
const talent = ref<Talent>();

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("talents.title"), to: { name: "Talents" } }));
const hasChanges = computed<boolean>(() =>
  Boolean(
    talent.value &&
    (talent.value.name !== name.value ||
      talent.value.allowMultiplePurchases !== allowMultiplePurchases.value ||
      (talent.value.skill ?? "") !== skill.value ||
      (talent.value.requiredTalent?.id ?? "") !== (requiredTalent.value?.id ?? "") ||
      (talent.value.summary ?? "") !== summary.value ||
      (talent.value.content ?? "") !== content.value),
  ),
);
const title = computed<string>(() => talent.value?.name ?? "");

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && talent.value) {
    isLoading.value = true;
    try {
      const payload: CreateOrReplaceTalentPayload = {
        tier: talent.value.tier,
        name: name.value,
        summary: summary.value,
        content: content.value,
        allowMultiplePurchases: allowMultiplePurchases.value,
        skill: skill.value ? (skill.value as Skill) : undefined,
        requiredTalentId: requiredTalent.value?.id,
      };
      talent.value = await replaceTalent(talent.value.id, payload);
      reinitialize();
      toasts.success("saved");
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  talent,
  (talent) => {
    name.value = talent?.name ?? "";
    allowMultiplePurchases.value = talent?.allowMultiplePurchases ?? false;
    skill.value = talent?.skill ?? "";
    requiredTalent.value = talent?.requiredTalent ? { ...talent.requiredTalent } : undefined;
    summary.value = talent?.summary ?? "";
    content.value = talent?.content ?? "";
  },
  { deep: true },
);

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    talent.value = await readTalent(id);
    document.setTitle(title.value);
  } catch (e: unknown) {
    const failure = e as ApiFailure;
    if (failure.status === StatusCodes.NotFound) {
      router.push("/not-found");
    } else {
      handleError(e);
    }
  }
});
</script>
