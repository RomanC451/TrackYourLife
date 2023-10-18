import { sidebarSectionsEnum } from "~/components/sidebar/sideBarSections";

export enum SideBarActionsEnum {
  toggleSideBar,
  setActiveElement
}

type ToggleSideBar = {
  type: SideBarActionsEnum.toggleSideBar;
};

type SetActiveElement = {
  type: SideBarActionsEnum.setActiveElement;
  payload: sidebarSectionsEnum;
};

export type SideBarActionsTypes = ToggleSideBar | SetActiveElement;
