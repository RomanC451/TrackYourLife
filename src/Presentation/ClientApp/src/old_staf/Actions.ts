import { sidebarSectionsEnum } from "~/components/sidebar/components/sideBarLinks";

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
