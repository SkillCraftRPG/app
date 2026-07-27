import { urlUtils } from "logitar-js";

import type { Profile, SignInAccountRequest, SignInAccountResponse, UpdateProfilePayload } from "@/types/account";
import { _delete, get, patch, post } from "./index";

export async function getProfile(): Promise<Profile> {
  const url: string = new urlUtils.UrlBuilder({ path: "/profile" }).buildRelative();
  return (await get<Profile>(url)).data;
}

export async function signIn(request: SignInAccountRequest): Promise<SignInAccountResponse> {
  const url: string = new urlUtils.UrlBuilder({ path: "/sign/in" }).buildRelative();
  return (await post<SignInAccountRequest, SignInAccountResponse>(url, request)).data;
}

export async function signOut(sessionId?: string): Promise<void> {
  if (sessionId) {
    const url: string = new urlUtils.UrlBuilder({ path: "/sessions/{sessionId}" }).setParameter("sessionId", sessionId).buildRelative();
    await _delete(url);
  } else {
    const url: string = new urlUtils.UrlBuilder({ path: "/sign/out" }).buildRelative();
    await post(url);
  }
}

export async function saveProfile(payload: UpdateProfilePayload): Promise<Profile> {
  const url: string = new urlUtils.UrlBuilder({ path: "/profile" }).buildRelative();
  return (await patch<UpdateProfilePayload, Profile>(url, payload)).data;
}
