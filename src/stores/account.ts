import { defineStore } from "pinia";
import { ref } from "vue";

import type { CurrentUser } from "@/types/account";

export const useAccountStore = defineStore(
  "account",
  () => {
    const currentUser = ref<CurrentUser>();
    const signedOut = ref<boolean>(false);

    function signIn(value: CurrentUser): void {
      currentUser.value = value;
    }
    function signOut(): void {
      currentUser.value = undefined;
      signedOut.value = true;
    }

    return { currentUser, signedOut, signIn, signOut };
  },
  {
    persist: true,
  },
);
