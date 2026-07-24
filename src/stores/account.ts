import { defineStore } from "pinia";
import { ref } from "vue";

import type { CurrentUser, Profile, SignOutEvent } from "@/types/account";

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

    function saveProfile(profile: Profile): void {
      if (currentUser.value) {
        currentUser.value.displayName = profile.fullName;
        currentUser.value.emailAddress = profile.emailAddress;
        currentUser.value.defaultExperience = profile.defaultExperience;
      } else {
        currentUser.value = {
          displayName: profile.fullName,
          emailAddress: profile.emailAddress,
          defaultExperience: profile.defaultExperience,
        };
      }
    }

    function signIn(value: CurrentUser): void {
      currentUser.value = value;
    }

    function signOut(e: SignOutEvent): void {
      currentUser.value = undefined;
      signedOutEvent.value = e;
    }

    return { currentUser, signedOutEvent, consumeSignOutEvent, saveProfile, signIn, signOut };
  },
  {
    persist: true,
  },
);
