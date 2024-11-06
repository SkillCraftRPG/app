<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import BackButton from "@/components/shared/BackButton.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import NameInput from "@/components/shared/NameInput.vue";
import SaveButton from "@/components/shared/SaveButton.vue";
import SkillSelect from "@/components/game/SkillSelect.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import TalentSelect from "@/components/talents/TalentSelect.vue";
import TierSelect from "@/components/shared/TierSelect.vue";
import type { ApiError } from "@/types/api";
import type { CreateOrReplaceTalentPayload, TalentModel } from "@/types/talents";
import type { Skill } from "@/types/game";
import { handleErrorKey } from "@/inject/App";
import { readTalent, replaceTalent } from "@/api/talents";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const allowMultiplePurchases = ref<boolean>(false);
const description = ref<string>("");
const hasLoaded = ref<boolean>(false);
const name = ref<string>("");
const requiredTalent = ref<TalentModel>();
const skill = ref<Skill>();
const talent = ref<TalentModel>();

const hasChanges = computed<boolean>(
  () =>
    !!talent.value &&
    (name.value !== talent.value.name ||
      skill.value !== (talent.value.skill ?? undefined) ||
      requiredTalent.value?.id !== talent.value.requiredTalent?.id ||
      allowMultiplePurchases.value !== talent.value.allowMultiplePurchases ||
      description.value !== (talent.value.description ?? "")),
);

function setModel(model: TalentModel): void {
  talent.value = model;
  allowMultiplePurchases.value = model.allowMultiplePurchases;
  description.value = model.description ?? "";
  name.value = model.name;
  requiredTalent.value = model.requiredTalent ?? undefined;
  skill.value = model.skill ?? undefined;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (talent.value) {
    try {
      const payload: CreateOrReplaceTalentPayload = {
        tier: talent.value.tier,
        name: name.value,
        description: description.value,
        allowMultiplePurchases: allowMultiplePurchases.value,
        requiredTalentId: requiredTalent.value?.id,
        skill: skill.value || undefined,
      };
      const model: TalentModel = await replaceTalent(talent.value.id, payload, talent.value.version);
      setModel(model);
      toasts.success("talents.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const talent: TalentModel = await readTalent(id);
      setModel(talent);
    }
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (status === 404) {
      router.push({ path: "/not-found" });
    } else {
      handleError(e);
    }
  }
  hasLoaded.value = true;
});
</script>

<template>
  <main class="container">
    <template v-if="talent">
      <h1>{{ talent.name }}</h1>
      <AppBreadcrumb :current="talent.name" :parent="{ route: { name: 'TalentList' }, text: t('talents.list') }" :world="talent.world" @error="handleError" />
      <StatusDetail :aggregate="talent" />
      <form @submit.prevent="onSubmit">
        <div class="row">
          <NameInput class="col-lg-6" required v-model="name" />
          <TierSelect class="col-lg-6" disabled :model-value="talent.tier" validation="server" />
        </div>
        <div class="row">
          <SkillSelect class="col-lg-6" v-model="skill" />
          <TalentSelect
            class="col-lg-6"
            label="talents.required"
            :max-tier="talent.tier"
            :model-value="requiredTalent?.id"
            placeholder="talents.select.none"
            @error="handleError"
            @selected="requiredTalent = $event"
          />
        </div>
        <TarCheckbox class="mb-3" id="allow-multiple-purchases" :label="t('talents.allowMultiplePurchases')" v-model="allowMultiplePurchases" />
        <DescriptionTextarea v-model="description" />
        <div>
          <SaveButton class="me-1" :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
          <BackButton class="ms-1" :has-changes="hasChanges" />
        </div>
      </form>
    </template>
  </main>
</template>
