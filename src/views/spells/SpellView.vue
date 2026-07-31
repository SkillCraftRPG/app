<template>
  <main class="container page">
    <div v-if="spell">
      <div class="d-flex flex-wrap align-items-center gap-2">
        <h1>{{ title }}</h1>
        <SpellTierDisplay class="fs-6" :tier="spell.tier" />
      </div>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("spells.created.lead") }}</strong> {{ t("spells.created.help", { name: title }) }}
      </TarAlert>
      <StatusDetail class="mb-3" :subject="spell" />
      <form class="border-top border-secondary-subtle pt-4" @submit.prevent="handleSubmit(submit)">
        <NameField class="mb-3" required v-model="name" />
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
import SpellTierDisplay from "@/components/spells/SpellTierDisplay.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarButton from "@/components/tar/TarButton.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { CreateOrReplaceSpellPayload, Spell } from "@/types/spells";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readSpell, replaceSpell } from "@/api/spells";
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

const content = ref<string>("");
const isCreated = ref<boolean>(events.shift() === "created");
const isLoading = ref<boolean>(false);
const name = ref<string>("");
const spell = ref<Spell>();
const summary = ref<string>("");

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("spells.title"), to: { name: "Spells" } }));
const hasChanges = computed<boolean>(() =>
  Boolean(spell.value && (spell.value.name !== name.value || (spell.value.summary ?? "") !== summary.value || (spell.value.content ?? "") !== content.value)),
);
const title = computed<string>(() => spell.value?.name ?? "");

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && spell.value) {
    isLoading.value = true;
    try {
      const payload: CreateOrReplaceSpellPayload = {
        tier: spell.value.tier,
        name: name.value,
        summary: summary.value,
        content: content.value,
      };
      spell.value = await replaceSpell(spell.value.id, payload);
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
  spell,
  (spell) => {
    name.value = spell?.name ?? "";
    summary.value = spell?.summary ?? "";
    content.value = spell?.content ?? "";
  },
  { deep: true },
);

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    spell.value = await readSpell(id);
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
