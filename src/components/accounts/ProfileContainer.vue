<template>
  <div>
    <div class="row text-body-secondary small">
      <div class="col-md-6">
        <div class="mb-3">
          <strong>{{ t("account.createdOn") }}</strong>
          <br />
          {{ d(modelValue.createdOn, "medium") }}
        </div>
      </div>
      <div class="col-md-6">
        <div class="mb-3">
          <strong>{{ t("account.updatedOn") }}</strong>
          <br />
          {{ d(modelValue.updatedOn, "medium") }}
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col-md-6">
        <EmailDisplay class="mb-3" :email="email" />
      </div>
      <div class="col-md-6">
        <div class="mb-3">
          <strong>{{ t("account.multiFactorAuthentication.label") }}</strong>
          <br />
          <MultiFactorAuthenticationDisplay :mode="modelValue.multiFactorAuthenticationMode" />
        </div>
      </div>
    </div>
    <ProfileForm :model-value="modelValue" @error="$emit('error', $event)" @update:model-value="$emit('update:model-value', $event)" />
    <div class="mb-3">
      <strong>TODO(fpion): 🚧</strong>
      <br />
      {{ modelValue.defaultExperience }}
    </div>
    <div class="mb-3">
      <div v-if="modelValue.passwordChangedOn">{{ d(modelValue.passwordChangedOn, "medium") }}</div>
      <div v-else>TODO(fpion): 🚧</div>
    </div>
    <div class="mb-3">
      <div v-if="modelValue.authenticatedOn">{{ d(modelValue.authenticatedOn, "medium") }}</div>
      <div v-else>TODO(fpion): 🚧</div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import EmailDisplay from "./EmailDisplay.vue";
import MultiFactorAuthenticationDisplay from "./MultiFactorAuthenticationDisplay.vue";
import ProfileForm from "./ProfileForm.vue";
import type { Email, Profile } from "@/types/account";
import { computed } from "vue";

const { d, t } = useI18n();

const props = defineProps<{
  modelValue: Profile;
}>();

defineEmits<{
  (e: "error", value: unknown): void;
  (e: "update:model-value", value: Profile): void;
}>();

const email = computed<Email>(() => ({ address: props.modelValue.emailAddress, isVerified: true }));
</script>
