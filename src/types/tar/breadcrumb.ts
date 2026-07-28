import type { RouteLocationAsPathGeneric, RouteLocationAsRelativeGeneric } from "vue-router";

/**
 * The options of a breadcrumb inside the TarBreadcrumb component.
 */
export type Breadcrumb = {
  /**
   * The text to display inside the breadcrumb.
   */
  text: string;
  /**
   * Route Location the link should navigate to when clicked on.
   */
  to?: string | RouteLocationAsRelativeGeneric | RouteLocationAsPathGeneric;
};

/**
 * The options of the TarBreadcrumb component.
 */
export type BreadcrumbOptions = {
  /**
   * The accessibility label describing the breadcrumbs.
   */
  ariaLabel?: string;
  /**
   * The list of breadcrumbs to display.
   */
  breadcrumbs?: Breadcrumb[];
  /**
   * The divider character or string that will be displayed between breadcrumbs.
   */
  divider?: string;
};
