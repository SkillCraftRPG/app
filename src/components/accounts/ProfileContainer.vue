<template>
  <div>
    <div class="row text-body-secondary small">
      <div class="col-md-6">
        <div class="mb-3">
          <div class="fw-bold">{{ t("account.createdOn") }}</div>
          <div>{{ d(modelValue.createdOn, "medium") }}</div>
        </div>
      </div>
      <div class="col-md-6">
        <div class="mb-3">
          <div class="fw-bold">{{ t("account.updatedOn") }}</div>
          <div>{{ d(modelValue.updatedOn, "medium") }}</div>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col-md-6">
        <EmailDisplay class="mb-3" :email="email" />
      </div>
    </div>
    <ProfileForm :model-value="modelValue" @error="$emit('error', $event)" @update:model-value="$emit('update:model-value', $event)" />
    <h2 class="h3">{{ t("account.profile.security.label") }}</h2>
    <div class="row">
      <div class="col-md-6">
        <div class="mb-3">
          <div class="fw-bold">{{ t("account.password.label") }}</div>
          <div v-if="modelValue.passwordChangedOn">{{ t("account.password.changedOn") }}&nbsp;{{ d(modelValue.passwordChangedOn, "medium") }}</div>
          <div v-else class="text-body-secondary">{{ t("account.password.none") }}</div>
        </div>
      </div>
      <div class="col-md-6">
        <div class="mb-3">
          <div class="fw-bold">{{ t("account.multiFactorAuthentication.label") }}</div>
          <MultiFactorAuthenticationDisplay :mode="modelValue.multiFactorAuthenticationMode" />
        </div>
      </div>
    </div>
    <div v-if="modelValue.authenticatedOn" class="mb-3">
      <div class="fw-bold">{{ t("account.authenticatedOn") }}</div>
      <div>{{ d(modelValue.authenticatedOn, "medium") }}</div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import EmailDisplay from "./EmailDisplay.vue";
import MultiFactorAuthenticationDisplay from "./MultiFactorAuthenticationDisplay.vue";
import ProfileForm from "./ProfileForm.vue";
import type { Email, Profile, Session } from "@/types/account";
import { listSessions } from "@/api/sessions";
import type { SearchResults } from "@/types/search";

const { d, t } = useI18n();

const props = defineProps<{
  modelValue: Profile;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "update:model-value", value: Profile): void;
}>();

const sessions = ref<SearchResults<Session>>({ items: [], total: 0 });

const email = computed<Email>(() => ({ address: props.modelValue.emailAddress, isVerified: true }));

onMounted(async () => {
  try {
    sessions.value = await listSessions();
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>
