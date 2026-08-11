export type Actor = {
  realmId?: string | null;
  type: ActorType;
  id: string;
  isDeleted: boolean;
  displayName: string;
  emailAddress?: string | null;
  pictureUrl?: string | null;
};

export type ActorType = "ApiKey" | "System" | "User";

export type Aggregate = Auditable & {
  id: string;
  version: number;
};

export type ApiError = {
  code: string;
  message: string;
  data: Record<string, unknown>;
};

export type ApiFailure = {
  data?: unknown;
  status: number;
};

export type ApiResult<T> = {
  data: T;
  status: number;
};

export type ApiVersion = {
  title: string;
  version: string;
};

export type Auditable = {
  createdBy: Actor;
  createdOn: string;
  updatedBy: Actor;
  updatedOn: string;
};

export enum ErrorCodes {
  InvalidCredentials = "InvalidCredentials",
  KeyAlreadyUsed = "KeyAlreadyUsed",
}

export type Optional<T> = {
  value?: T | null;
};

export type ProblemDetails = {
  type?: string | null;
  title?: string | null;
  status?: number | null;
  detail?: string | null;
  instance?: string | null;
  error?: ApiError | null;
};

export enum StatusCodes {
  BadRequest = 400,
  Unauthorized = 401,
  NotFound = 404,
  Conflict = 409,
}

export const SYSTEM: Actor = {
  type: "System",
  id: "00000000-0000-0000-0000-000000000000",
  isDeleted: false,
  displayName: "System",
};
