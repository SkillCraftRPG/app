export type AuthenticationFlow = "Password" | "Passwordless";

export type CompleteProfilePayload = {
  token: string;
  password?: string | null;
  multiFactorAuthenticationMode: MultiFactorAuthenticationMode;
  firstName: string;
  lastName: string;
  dateOfBirth?: Date | null;
  gender?: Gender | null;
  locale: string;
  timeZone: string;
};

export type Credentials = {
  locale: string;
  emailAddress: string;
  password?: string | null;
  usePasswordless: boolean;
};

export type CurrentUser = {
  displayName: string;
  emailAddress?: string | null;
  pictureUrl?: string | null;
};

export type Gender = "Male" | "Female" | "Other";

export type MultiFactorAuthenticationMessage = {
  oneTimePasswordId: string;
  messageId: string;
  multiFactorAuthenticationMode: MultiFactorAuthenticationMode;
};

export type MultiFactorAuthenticationMode = "None" | "Email" | "Phone";

export type OneTimePasswordValidation = {
  id: string;
  code: string;
};

export type SignInAccountRequest = {
  credentials?: Credentials | null;
  authenticationToken?: string | null;
  oneTimePassword?: OneTimePasswordValidation | null;
  profile?: CompleteProfilePayload | null;
};

export type SignInAccountResponse = {
  allowedFlows: AuthenticationFlow[];
  emailVerificationMessageId?: string | null;
  multiFactorAuthenticationMessage?: MultiFactorAuthenticationMessage | null;
  profileCompletionToken?: string | null;
  currentUser?: CurrentUser | null;
};
