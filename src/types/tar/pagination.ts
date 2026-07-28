/**
 * The options of the TarPagination component.
 */
export type PaginationOptions = {
  /**
   * The accessibility label describing the "first" button.
   */
  ariaFirst?: string;
  /**
   * The accessibility label describing the pagination.
   */
  ariaLabel?: string;
  /**
   * The accessibility label describing the "last" button.
   */
  ariaLast?: string;
  /**
   * The accessibility label describing the "next" button.
   */
  ariaNext?: string;
  /**
   * The accessibility label describing the "previous" button.
   */
  ariaPrevious?: string;
  /**
   * The number of items per page.
   */
  count?: number;
  /**
   * The characters to display in the "first" button. If unspecified, no "first" button will be displayed.
   */
  first?: string;
  /**
   * The characters to display in the "last" button. If unspecified, no "last" button will be displayed.
   */
  last?: string;
  /**
   * The number of the current page.
   */
  modelValue?: number;
  /**
   * The characters to display in the "next" button. If unspecified, no "next" button will be displayed.
   */
  next?: string;
  /**
   * The number of pages to display.
   */
  pages?: number;
  /**
   * The alignment of the pagination in its container.
   */
  position?: PaginationPosition;
  /**
   * The characters to display in the "previous" button. If unspecified, no "previous" button will be displayed.
   */
  previous?: string;
  /**
   * The size of the pagination.
   */
  size?: PaginationSize;
  /**
   * The total number of items.
   */
  total?: number;
};

/**
 * The positions of the TarPagination component.
 */
export type PaginationPosition = "left" | "center" | "right";

/**
 * The sizes of the TarPagination component.
 */
export type PaginationSize = "small" | "medium" | "large";
