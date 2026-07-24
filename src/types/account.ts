import type { Locale } from "./i18n";
import type { Optional } from "./api";

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
  defaultExperience: UserExperience;
};

export type Contact = {
  isVerified: boolean;
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
  defaultExperience: UserExperience;
};

export type Email = Contact & {
  address: string;
};

export type Gender = "male" | "female" | "other";

export type MultiFactorAuthenticationChallenge = {
  oneTimePasswordId: string;
  messageId: string;
  mode: MultiFactorAuthenticationMode;
  maskedContact: string;
};

export type MultiFactorAuthenticationMode = "None" | "Email" | "Phone";

export type OneTimePasswordValidation = {
  id: string;
  code: string;
};

export type PersonalInformation = {
  firstName: string;
  lastName: string;
};

export type PreferencesInformation = {
  dateOfBirth?: Date | null;
  gender?: Gender | null;
  locale: string;
  timeZone: string;
};

export type Profile = {
  emailAddress: string;
  passwordChangedOn?: string | null;
  multiFactorAuthenticationMode: MultiFactorAuthenticationMode;
  firstName: string;
  lastName: string;
  fullName: string;
  dateOfBirth?: string | null;
  gender?: Gender;
  locale: Locale;
  timeZone: string;
  createdOn: string;
  updatedOn: string;
  authenticatedOn?: string | null;
  defaultExperience: UserExperience;
};

export type SecurityInformation = {
  mode: SecurityMode;
  password: string;
};

export type SecurityMode = "PasswordLess" | "Password" | "MultiFactor";

export type SignInAccountRequest = {
  credentials?: Credentials | null;
  authenticationToken?: string | null;
  oneTimePassword?: OneTimePasswordValidation | null;
  profile?: CompleteProfilePayload | null;
};

export type SignInAccountResponse = {
  allowedFlows: AuthenticationFlow[];
  emailVerificationMessageId?: string | null;
  multiFactorAuthenticationChallenge?: MultiFactorAuthenticationChallenge | null;
  profileCompletionToken?: string | null;
  currentUser?: CurrentUser | null;
};

export type SignOutEvent = "closed" | "expired";

export type TimeZone = {
  id: string;
  displayName: string;
};

export type TokenPayload = {
  email?: string | null;
  email_verified?: boolean | string | null;
};

export type UpdateProfilePayload = {
  firstName?: string | null;
  lastName?: string | null;
  dateOfBirth?: Optional<Date> | null;
  gender?: Optional<Gender> | null;
  locale?: string | null;
  timeZone?: string | null;
  defaultExperience?: UserExperience | null;
};

export type UserExperience = "Player" | "Gamemaster";
