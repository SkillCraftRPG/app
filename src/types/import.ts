export type ImportData<T> = {
  existing?: T;
  reference: T;
  selected: boolean;
  status: ImportStatus;
};

export type ImportStatus = "NotImported" | "Outdated" | "UpToDate";
