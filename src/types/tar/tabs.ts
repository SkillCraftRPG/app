import type { InjectionKey } from "vue";

/**
 * The injection key to bind a tab to its container.
 */
export const bindTabKey = Symbol() as InjectionKey<(tab: TabOptions) => void>;

/**
 * The injection key to unbind a tab from its container.
 */
export const unbindTabKey = Symbol() as InjectionKey<(tab: TabOptions) => void>;

/**
 * The options of the TarTabs component.
 */
export type TabContainerOptions = {
  /**
   * The unique identifier of the tab container.
   */
  id?: string;
};

/**
 * The options of the TarTab component.
 */
export type TabOptions = {
  /**
   * This tab contents will be displayed initially.
   */
  active?: boolean | string;
  /**
   * The tab header will be disabled, so clicking it will not display the tab contents.
   */
  disabled?: boolean | string;
  /**
   * The unique identifier of the tab. A random `nanoid` will be generated if unspecified.
   */
  id?: string;
  /**
   * The header title of the tab.
   */
  title: string;
};
