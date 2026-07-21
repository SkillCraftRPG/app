/**
 * The options of the TarTextarea component.
 */
export type TextareaOptions = {
  /**
   * The visible width of the textarea, in average character widths.
   */
  cols?: number | string;
  /**
   * When adding a textarea help text, this is the unique identifier of the form text element. This will specify the `aria-describedby` attribute to ensure assistive technologies will announce the text when the user focuses or enters the control.
   */
  describedBy?: string;
  /**
   * The textarea will display with a disabled style and will not react to events.
   */
  disabled?: boolean | string;
  /**
   * The label will appear floating in the textarea, moving when the value is modified. The `label` and `placeholder` properties are required for floating labels to function properly.
   */
  floating?: boolean | string;
  /**
   * The unique identifier of the textarea.
   */
  id?: string;
  /**
   * The human readable caption of the textarea.
   */
  label?: string;
  /**
   * The maximum length in characters of a valid value.
   */
  max?: number | string;
  /**
   * The minimum length in characters of a valid value.
   */
  min?: number | string;
  /**
   * The value of the field.
   */
  modelValue?: string;
  /**
   * The name of the textarea, used when submitting forms.
   */
  name?: string;
  /**
   * This text will appear inside the textarea when no value has been set.
   */
  placeholder?: string;
  /**
   * When the textarea is readonly, this will display the value as text instead of a textarea, preserving margin and padding, but removing form field styling.
   */
  plaintext?: boolean | string;
  /**
   * The value of the textarea will not be editable.
   */
  readonly?: boolean | string;
  /**
   * The textarea is required to submit the form its contained into. If the value is `label`, then the textarea label will appear as required, but the textarea itself will not have the required attribute. This is useful for client-validated forms.
   */
  required?: boolean | string;
  /**
   * The number of visible text lines of the textarea.
   */
  rows?: number | string;
  /**
   * The size of the textarea.
   */
  size?: TextareaSize;
  /**
   * The status of the textarea. The textarea will display a valid style (green border) or invalid style (red border) when specified.
   */
  status?: TextareaStatus;
};

/**
 * The sizes of the TarTextarea component.
 */
export type TextareaSize = "small" | "medium" | "large";

/**
 * The status values of the TarTextarea component.
 */
export type TextareaStatus = "invalid" | "valid";
