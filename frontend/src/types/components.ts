import type { RouteLocationAsPathGeneric, RouteLocationAsRelativeGeneric } from "vue-router";

export type Breadcrumb = {
  route?: string | RouteLocationAsRelativeGeneric | RouteLocationAsPathGeneric;
  text: string;
};
