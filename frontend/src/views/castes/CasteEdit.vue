<script setup lang="ts">
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
import TraitList from "@/components/castes/TraitList.vue";
import WealthRollInput from "@/components/castes/WealthRollInput.vue";
import type { ApiError } from "@/types/api";
import type { CreateOrReplaceCastePayload, CasteModel, TraitStatus, TraitPayload, TraitUpdated } from "@/types/castes";
import type { Skill } from "@/types/game";
import { handleErrorKey } from "@/inject/App";
import { readCaste, replaceCaste } from "@/api/castes";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const caste = ref<CasteModel>();
const description = ref<string>("");
const hasLoaded = ref<boolean>(false);
const name = ref<string>("");
const skill = ref<Skill>();
const traits = ref<TraitStatus[]>([]);
const wealthRoll = ref<string>("");

const hasChanges = computed<boolean>(
  () =>
    !!caste.value &&
    (name.value !== caste.value.name ||
      skill.value !== (caste.value.skill ?? undefined) ||
      wealthRoll.value !== (caste.value.wealthRoll ?? "") ||
      description.value !== (caste.value.description ?? "") ||
      traits.value.some((trait) => !trait.trait.id || trait.isRemoved || trait.isUpdated)),
);

function setModel(model: CasteModel): void {
  caste.value = model;
  description.value = model.description ?? "";
  name.value = model.name;
  skill.value = model.skill ?? undefined;
  traits.value = model.traits.map((trait) => ({ trait: { ...trait }, isRemoved: false, isUpdated: false }));
  wealthRoll.value = model.wealthRoll ?? "";
}

function onTraitAdded(trait: TraitPayload): void {
  traits.value.push({ trait, isRemoved: false, isUpdated: false });
}
function onTraitRemoved(index: number): void {
  const trait: TraitStatus | undefined = traits.value[index];
  if (trait) {
    if (trait.trait.id) {
      trait.isRemoved = !trait.isRemoved;
    } else {
      traits.value.splice(index, 1);
    }
  }
}
function onTraitUpdated(event: TraitUpdated): void {
  const trait: TraitStatus | undefined = traits.value[event.index];
  if (trait) {
    trait.trait = event.trait;
    if (trait.trait.id) {
      trait.isUpdated = true;
    }
  }
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (caste.value) {
    try {
      const payload: CreateOrReplaceCastePayload = {
        name: name.value,
        description: description.value,
        skill: skill.value,
        wealthRoll: wealthRoll.value,
        traits: traits.value.filter(({ isRemoved }) => !isRemoved).map(({ trait }) => trait),
      };
      const model: CasteModel = await replaceCaste(caste.value.id, payload, caste.value.version);
      setModel(model);
      toasts.success("castes.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const caste: CasteModel = await readCaste(id);
      setModel(caste);
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
    <template v-if="caste">
      <h1>{{ caste.name }}</h1>
      <AppBreadcrumb :current="caste.name" :parent="{ route: { name: 'CasteList' }, text: t('castes.list') }" :world="caste.world" @error="handleError" />
      <StatusDetail :aggregate="caste" />
      <form @submit.prevent="onSubmit">
        <div class="row">
          <NameInput class="col-lg-4" required v-model="name" />
          <SkillSelect class="col-lg-4" v-model="skill" />
          <WealthRollInput class="col-lg-4" v-model="wealthRoll" />
        </div>
        <DescriptionTextarea v-model="description" />
        <TraitList :traits="traits" @added="onTraitAdded" @removed="onTraitRemoved" @updated="onTraitUpdated" />
        <div>
          <SaveButton class="me-1" :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
          <BackButton class="ms-1" :has-changes="hasChanges" />
        </div>
      </form>
    </template>
  </main>
</template>
