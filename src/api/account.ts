import { urlUtils } from "logitar-js";

import type { SignInAccountRequest, SignInAccountResponse } from "@/types/account";
import { post } from "./index";

export async function signIn(request: SignInAccountRequest): Promise<SignInAccountResponse> {
  const url: string = new urlUtils.UrlBuilder({ path: "/sign/in" }).buildRelative();
  return (await post<SignInAccountRequest, SignInAccountResponse>(url, request)).data;
}
