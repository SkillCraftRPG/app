import { defineStore } from "pinia";
import { ref } from "vue";

import type { CurrentUser, SignOutEvent } from "@/types/account";

export const useAccountStore = defineStore(
  "account",
  () => {
    const currentUser = ref<CurrentUser>();
    const signedOutEvent = ref<SignOutEvent>();

    function consumeSignOutEvent(): SignOutEvent | undefined {
      const e: SignOutEvent | undefined = signedOutEvent.value;
      signedOutEvent.value = undefined;
      return e;
    }

    function signIn(value: CurrentUser): void {
      currentUser.value = value;
    }

    function signOut(e: SignOutEvent): void {
      currentUser.value = undefined;
      signedOutEvent.value = e;
    }

    return { currentUser, signedOutEvent, consumeSignOutEvent, signIn, signOut };
  },
  {
    persist: true,
  },
);
