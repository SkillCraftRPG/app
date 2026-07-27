import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { Session } from "@/types/account";
import { get } from "./index";

export async function listSessions(): Promise<SearchResults<Session>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/sessions" }).buildRelative();
  return (await get<SearchResults<Session>>(url)).data;
}

// Déconnecter toutes les sessions ?
// Vous serez déconnecté de tous vos appareils, y compris celui-ci. Vous devrez vous connecter de nouveau pour accéder à votre compte.

// Sign out of all sessions?
// You’ll be signed out on all your devices, including this one. You’ll need to sign in again to access your account.
