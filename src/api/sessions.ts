import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { Session } from "@/types/account";
import { _delete, get } from "./index";

export async function listActiveSessions(): Promise<SearchResults<Session>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/sessions" }).buildRelative();
  return (await get<SearchResults<Session>>(url)).data;
}

export async function signOutById(id: string): Promise<void> {
  const url: string = new urlUtils.UrlBuilder({ path: "/sessions/{id}" }).setParameter("id", id).buildRelative();
  await _delete(url);
}

// Déconnecter toutes les sessions ?
// Vous serez déconnecté de tous vos appareils, y compris celui-ci. Vous devrez vous connecter de nouveau pour accéder à votre compte.

// Sign out of all sessions?
// You’ll be signed out on all your devices, including this one. You’ll need to sign in again to access your account.
