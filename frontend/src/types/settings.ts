export type PasswordSettings = {
  minimumLength?: number;
  uniqueCharacters?: number;
  requireNonAlphanumeric?: boolean;
  requireLowercase?: boolean;
  requireUppercase?: boolean;
  requireDigit?: boolean;
};

export const passwordSettings: PasswordSettings = {
  minimumLength: 8,
  uniqueCharacters: 8,
  requireNonAlphanumeric: true,
  requireLowercase: true,
  requireUppercase: true,
  requireDigit: true,
};
