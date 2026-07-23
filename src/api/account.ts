import { urlUtils } from "logitar-js";

import type { Profile, SignInAccountRequest, SignInAccountResponse } from "@/types/account";
import { get, post } from "./index";

export async function getProfile(): Promise<Profile> {
  const url: string = new urlUtils.UrlBuilder({ path: "/profile" }).buildRelative();
  return (await get<Profile>(url)).data;
}

export async function signIn(request: SignInAccountRequest): Promise<SignInAccountResponse> {
  const url: string = new urlUtils.UrlBuilder({ path: "/sign/in" }).buildRelative();
  return (await post<SignInAccountRequest, SignInAccountResponse>(url, request)).data;
}
