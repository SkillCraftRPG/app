<template>
  <div class="row flex-grow-1 text-center align-items-center mx-0">
    <div class="col jumbotron mb-0">
      <h1 class="display-4">{{ t("account.multiFactorAuthentication.verify") }}</h1>
      <p class="lead" v-html="lead"></p>
      <div class="row justify-content-center">
        <form class="col-md-6" @submit.prevent="handleSubmit(submit)">
          <IncorrectOneTimeCode v-model="incorrectCode" />
          <InvalidOneTimeCode v-if="invalidCode" show />
          <FormInput
            v-if="!invalidCode"
            class="mb-3"
            floating
            id="code"
            :label="label"
            min="6"
            max="6"
            pattern="[0-9]{6}"
            :placeholder="label"
            ref="codeInput"
            required
            v-model="code"
          />
          <div class="d-flex gap-3">
            <TarButton class="flex-fill" icon="fas fa-xmark" outline size="large" :text="t('actions.cancel')" variant="secondary" @click="$emit('cancel')" />
            <TarButton
              v-if="!invalidCode"
              class="flex-fill"
              :disabled="isLoading"
              icon="fas fa-check"
              :loading="isLoading"
              size="large"
              :text="t('actions.verify')"
              type="submit"
            />
          </div>
        </form>
      </div>
      <hr class="my-4" />
      <p>
        <span class="text-body-secondary">{{ t("account.multiFactorAuthentication.reference") }}</span
        >&nbsp;<code class="text-body-secondary">{{ challenge.messageId }}</code>
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import IncorrectOneTimeCode from "./IncorrectOneTimeCode.vue";
import InvalidOneTimeCode from "./InvalidOneTimeCode.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { MultiFactorAuthenticationChallenge, SignInAccountRequest, SignInAccountResponse } from "@/types/account";
import { ErrorCodes, StatusCodes, type ApiFailure, type ProblemDetails } from "@/types/api";
import { signIn } from "@/api/account";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  challenge: MultiFactorAuthenticationChallenge;
}>();

const emit = defineEmits<{
  (e: "cancel"): void;
  (e: "error", value: unknown): void;
  (e: "response", value: SignInAccountResponse): void;
}>();

const code = ref<string>("");
const codeInput = ref<InstanceType<typeof FormInput> | null>(null);
const failedAttempts = ref<number>(0);
const incorrectCode = ref<boolean>(false);
const isLoading = ref<boolean>(false);

const invalidCode = computed<boolean>(() => failedAttempts.value === 5);
const label = computed<string>(() => t("account.multiFactorAuthentication.code"));
const lead = computed<string>(() =>
  t(`account.multiFactorAuthentication.${props.challenge.mode.toLowerCase()}.help`, { contact: props.challenge.maskedContact }),
);

const { handleSubmit } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    incorrectCode.value = false;
    try {
      const request: SignInAccountRequest = {
        oneTimePassword: {
          id: props.challenge.oneTimePasswordId,
          code: code.value,
        },
      };
      const response: SignInAccountResponse = await signIn(request);
      emit("response", response);
    } catch (e: unknown) {
      const failure = e as ApiFailure;
      if (failure.status === StatusCodes.BadRequest) {
        const problemDetails = failure.data as ProblemDetails;
        if (problemDetails.error && problemDetails.error.code === ErrorCodes.InvalidCredentials) {
          failedAttempts.value++;
          if (!invalidCode.value) {
            incorrectCode.value = true;
            code.value = "";
            codeInput.value?.focus();
          }
          return;
        }
      }
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

onMounted(() => codeInput.value?.focus());

// TODO(fpion): input should have `inputmode="numeric"`, `autocomplete="one-time-code"` and enterkeyhint="done".
</script>
