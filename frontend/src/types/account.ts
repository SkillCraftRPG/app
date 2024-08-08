export type CompleteProfilePayload = SaveProfilePayload & {
  token: string;
  password?: string;
};

export type ContactType = "Email" | "Phone";

export type Credentials = {
  emailAddress: string;
  password?: string;
};

export type CurrentUser = {
  displayName: string;
  emailAddress?: string;
  pictureUrl?: string;
};

export type Identification = {
  firstName: string;
  middleName?: string;
  lastName: string;
};

export type JwtPayload = {
  email?: string;
};

export type MultiFactorAuthenticationMode = "None" | "Email" | "Phone";

export type OneTimePasswordPayload = {
  id: string;
  code: string;
};

export type OneTimePasswordValidation = {
  id: string;
  sentMessage: SentMessage;
};

export type PersonNameType = "first" | "last" | "middle" | "nick";

export type PersonalInformation = {
  birthdate?: Date;
  gender?: string;
  locale: string;
  timeZone: string;
};

export type SaveProfilePayload = {
  multiFactorAuthenticationMode: MultiFactorAuthenticationMode;
  firstName: string;
  middleName?: string;
  lastName: string;
  birthdate?: Date;
  gender?: string;
  locale: string;
  timeZone: string;
  picture?: string;
  userType: UserType;
};

export type SecurityInformation = {
  password?: string;
  multiFactorAuthenticationMode: MultiFactorAuthenticationMode;
};

export type SentMessage = {
  contactType: ContactType;
  maskedContact: string;
  confirmationNumber: string;
};

export type SignInPayload = {
  locale: string;
  credentials?: Credentials;
  authenticationToken?: string;
  googleIdToken?: string;
  oneTimePassword?: OneTimePasswordPayload;
  profile?: CompleteProfilePayload;
};

export type SignInResponse = {
  authenticationLinkSentTo?: SentMessage;
  isPasswordRequired: boolean;
  oneTimePasswordValidation?: OneTimePasswordValidation;
  profileCompletionToken?: string;
  currentUser?: CurrentUser;
};

export type UserType = "Player" | "Gamemaster";
