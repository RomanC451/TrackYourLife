// function reducer(state: sideBarState, action: SideBarActionsTypes) {
//   switch (action.type) {
//     case SideBarActionsEnum.toggleSideBar:
//       return {
//         ...state,
//         sideBarOpened: !state.sideBarOpened
//       };
//     case SideBarActionsEnum.setActiveElement:
//       return {
//         ...state,
//         activeElement: action.payload
//       };

//     default:
//       throw new Error("Wrong action type.");
//   }
// }

// const [state, dispatch] = useReducer(reducer, {
//   activeElement: getSidebarActiveElement(location),
//   sideBarOpened: false
// });
