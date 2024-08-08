<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import EmailAddressInput from "@/components/users/EmailAddressInput.vue";
import PasswordInput from "@/components/users/PasswordInput.vue";
import type { Credentials, SignInPayload, SignInResponse } from "@/types/account";
import { signIn } from "@/api/account";

const { locale, t } = useI18n();

const props = defineProps<{
  response?: SignInResponse;
}>();

const credentials = ref<Credentials>({ emailAddress: "" });

const isPasswordRequired = computed<boolean>(() => props.response?.isPasswordRequired ?? false);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "response", value: SignInResponse): void;
}>();

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const payload: SignInPayload = {
      locale: locale.value,
      credentials: credentials.value,
    };
    const response: SignInResponse = await signIn(payload);
    emit("response", response);
  } catch (e: unknown) {
    emit("error", e); // ISSUE: https://github.com/SkillCraftRPG/app/issues/10
  }
});
</script>

<template>
  <div>
    <h1>{{ t("users.signIn.title") }}</h1>
    <!-- <TarAlert :close="t('actions.close')" dismissible variant="warning" v-model="invalidCredentials">
      <strong>{{ t("users.signIn.failed") }}</strong> {{ t("users.signIn.invalidCredentials") }}
    </TarAlert> -->
    <form @submit.prevent="onSubmit">
      <EmailAddressInput required v-model="credentials.emailAddress" />
      <PasswordInput v-if="isPasswordRequired" required v-model="credentials.password" />
      <TarButton
        :disabled="isSubmitting"
        icon="fas fa-arrow-right-to-bracket"
        :loading="isSubmitting"
        :status="t('loading')"
        :text="t('users.signIn.submit')"
        type="submit"
      />
    </form>
  </div>
</template>
