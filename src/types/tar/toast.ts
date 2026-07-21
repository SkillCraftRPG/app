/**
 * The horizontal alignments of toasts.
 */
export type HorizontalAlignment = "left" | "center" | "right";

/**
 * The vertical alignments of toasts.
 */
export type VerticalAlignment = "top" | "middle" | "bottom";

/**
 * The options of the TarToaster component.
 */
export type ToastContainerOptions = {
  /**
   * The horizontal alignment of toasts in this container.
   */
  horizontalAlignment?: HorizontalAlignment;
  /**
   * The Toasts to display inside the container.
   */
  toasts?: ToastOptions[];
  /**
   * The vertical alignment of toasts in this container.
   */
  verticalAlignment?: VerticalAlignment;
};

/**
 * The options of the TarToast component.
 */
export type ToastOptions = {
  /**
   * The close label for accessibility.
   */
  close?: string;
  /**
   * The duration of the toast, in milliseconds, after which it will be hidden automatically. If unspecified, or less or equal to 0, the toast will remain shown indefinitely.
   */
  duration?: number | string;
  /**
   * The toast will be animated using a fade transition when it will be hidden.
   */
  fade?: boolean | string;
  /**
   * The HTML to render inside the toast body. This property has precedence over the `text` property.
   */
  html?: string;
  /**
   * The unique identifier of the toast. A random `nanoid` will be generated if unspecified.
   */
  id?: string;
  /**
   * The background of the toast body will appear lightly darker.
   */
  solid?: boolean | string;
  /**
   * The text to render inside the toast body. This property is overridden by the `html` property.
   */
  text?: string;
  /**
   * The title that will appear in the toast header.
   */
  title?: string;
  /**
   * The color variant of the toast.
   */
  variant?: ToastVariant;
};

/**
 * The variants of the TarToast component.
 */
export type ToastVariant = "primary" | "secondary" | "success" | "danger" | "warning" | "info" | "light" | "dark";
