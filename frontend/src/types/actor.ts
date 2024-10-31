export type Actor = {
  id: string;
  type: ActorType;
  isDeleted: boolean;
  displayName: string;
  emailAddress?: string;
  pictureUrl?: string;
};

export type ActorType = "System" | "ApiKey" | "User";
