/* eslint-disable */

// @ts-nocheck

// noinspection JSUnusedGlobalSymbols

// This file was automatically generated by TanStack Router.
// You should NOT make any changes in this file as it will be overwritten.
// Additionally, you should also exclude this file from your linter and/or formatter to prevent it from being checked or modified.

// Import Routes

import { Route as rootRoute } from './routes/__root'
import { Route as AuthImport } from './routes/auth'
import { Route as AuthenticatedImport } from './routes/_authenticated'
import { Route as IndexImport } from './routes/index'
import { Route as AuthenticatedSidebarPageLayoutImport } from './routes/_authenticated/_sidebarPageLayout'
import { Route as AuthenticatedSidebarPageLayoutNavbarPageLayoutImport } from './routes/_authenticated/_sidebarPageLayout/_navbarPageLayout'
import { Route as AuthenticatedSidebarPageLayoutNavbarPageLayoutTestImport } from './routes/_authenticated/_sidebarPageLayout/_navbarPageLayout/test'
import { Route as AuthenticatedSidebarPageLayoutNavbarPageLayoutHomeImport } from './routes/_authenticated/_sidebarPageLayout/_navbarPageLayout/home'
import { Route as AuthenticatedSidebarPageLayoutNavbarPageLayoutDebugImport } from './routes/_authenticated/_sidebarPageLayout/_navbarPageLayout/debug'
import { Route as AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionRecipesImport } from './routes/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/recipes'
import { Route as AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionDiaryImport } from './routes/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/diary'

// Create/Update Routes

const AuthRoute = AuthImport.update({
  id: '/auth',
  path: '/auth',
  getParentRoute: () => rootRoute,
} as any)

const AuthenticatedRoute = AuthenticatedImport.update({
  id: '/_authenticated',
  getParentRoute: () => rootRoute,
} as any)

const IndexRoute = IndexImport.update({
  id: '/',
  path: '/',
  getParentRoute: () => rootRoute,
} as any)

const AuthenticatedSidebarPageLayoutRoute =
  AuthenticatedSidebarPageLayoutImport.update({
    id: '/_sidebarPageLayout',
    getParentRoute: () => AuthenticatedRoute,
  } as any)

const AuthenticatedSidebarPageLayoutNavbarPageLayoutRoute =
  AuthenticatedSidebarPageLayoutNavbarPageLayoutImport.update({
    id: '/_navbarPageLayout',
    getParentRoute: () => AuthenticatedSidebarPageLayoutRoute,
  } as any)

const AuthenticatedSidebarPageLayoutNavbarPageLayoutTestRoute =
  AuthenticatedSidebarPageLayoutNavbarPageLayoutTestImport.update({
    id: '/test',
    path: '/test',
    getParentRoute: () => AuthenticatedSidebarPageLayoutNavbarPageLayoutRoute,
  } as any)

const AuthenticatedSidebarPageLayoutNavbarPageLayoutHomeRoute =
  AuthenticatedSidebarPageLayoutNavbarPageLayoutHomeImport.update({
    id: '/home',
    path: '/home',
    getParentRoute: () => AuthenticatedSidebarPageLayoutNavbarPageLayoutRoute,
  } as any)

const AuthenticatedSidebarPageLayoutNavbarPageLayoutDebugRoute =
  AuthenticatedSidebarPageLayoutNavbarPageLayoutDebugImport.update({
    id: '/debug',
    path: '/debug',
    getParentRoute: () => AuthenticatedSidebarPageLayoutNavbarPageLayoutRoute,
  } as any)

const AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionRecipesRoute =
  AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionRecipesImport.update({
    id: '/nutrition/recipes',
    path: '/nutrition/recipes',
    getParentRoute: () => AuthenticatedSidebarPageLayoutNavbarPageLayoutRoute,
  } as any)

const AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionDiaryRoute =
  AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionDiaryImport.update({
    id: '/nutrition/diary',
    path: '/nutrition/diary',
    getParentRoute: () => AuthenticatedSidebarPageLayoutNavbarPageLayoutRoute,
  } as any)

// Populate the FileRoutesByPath interface

declare module '@tanstack/react-router' {
  interface FileRoutesByPath {
    '/': {
      id: '/'
      path: '/'
      fullPath: '/'
      preLoaderRoute: typeof IndexImport
      parentRoute: typeof rootRoute
    }
    '/_authenticated': {
      id: '/_authenticated'
      path: ''
      fullPath: ''
      preLoaderRoute: typeof AuthenticatedImport
      parentRoute: typeof rootRoute
    }
    '/auth': {
      id: '/auth'
      path: '/auth'
      fullPath: '/auth'
      preLoaderRoute: typeof AuthImport
      parentRoute: typeof rootRoute
    }
    '/_authenticated/_sidebarPageLayout': {
      id: '/_authenticated/_sidebarPageLayout'
      path: ''
      fullPath: ''
      preLoaderRoute: typeof AuthenticatedSidebarPageLayoutImport
      parentRoute: typeof AuthenticatedImport
    }
    '/_authenticated/_sidebarPageLayout/_navbarPageLayout': {
      id: '/_authenticated/_sidebarPageLayout/_navbarPageLayout'
      path: ''
      fullPath: ''
      preLoaderRoute: typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutImport
      parentRoute: typeof AuthenticatedSidebarPageLayoutImport
    }
    '/_authenticated/_sidebarPageLayout/_navbarPageLayout/debug': {
      id: '/_authenticated/_sidebarPageLayout/_navbarPageLayout/debug'
      path: '/debug'
      fullPath: '/debug'
      preLoaderRoute: typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutDebugImport
      parentRoute: typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutImport
    }
    '/_authenticated/_sidebarPageLayout/_navbarPageLayout/home': {
      id: '/_authenticated/_sidebarPageLayout/_navbarPageLayout/home'
      path: '/home'
      fullPath: '/home'
      preLoaderRoute: typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutHomeImport
      parentRoute: typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutImport
    }
    '/_authenticated/_sidebarPageLayout/_navbarPageLayout/test': {
      id: '/_authenticated/_sidebarPageLayout/_navbarPageLayout/test'
      path: '/test'
      fullPath: '/test'
      preLoaderRoute: typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutTestImport
      parentRoute: typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutImport
    }
    '/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/diary': {
      id: '/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/diary'
      path: '/nutrition/diary'
      fullPath: '/nutrition/diary'
      preLoaderRoute: typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionDiaryImport
      parentRoute: typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutImport
    }
    '/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/recipes': {
      id: '/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/recipes'
      path: '/nutrition/recipes'
      fullPath: '/nutrition/recipes'
      preLoaderRoute: typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionRecipesImport
      parentRoute: typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutImport
    }
  }
}

// Create and export the route tree

interface AuthenticatedSidebarPageLayoutNavbarPageLayoutRouteChildren {
  AuthenticatedSidebarPageLayoutNavbarPageLayoutDebugRoute: typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutDebugRoute
  AuthenticatedSidebarPageLayoutNavbarPageLayoutHomeRoute: typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutHomeRoute
  AuthenticatedSidebarPageLayoutNavbarPageLayoutTestRoute: typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutTestRoute
  AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionDiaryRoute: typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionDiaryRoute
  AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionRecipesRoute: typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionRecipesRoute
}

const AuthenticatedSidebarPageLayoutNavbarPageLayoutRouteChildren: AuthenticatedSidebarPageLayoutNavbarPageLayoutRouteChildren =
  {
    AuthenticatedSidebarPageLayoutNavbarPageLayoutDebugRoute:
      AuthenticatedSidebarPageLayoutNavbarPageLayoutDebugRoute,
    AuthenticatedSidebarPageLayoutNavbarPageLayoutHomeRoute:
      AuthenticatedSidebarPageLayoutNavbarPageLayoutHomeRoute,
    AuthenticatedSidebarPageLayoutNavbarPageLayoutTestRoute:
      AuthenticatedSidebarPageLayoutNavbarPageLayoutTestRoute,
    AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionDiaryRoute:
      AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionDiaryRoute,
    AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionRecipesRoute:
      AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionRecipesRoute,
  }

const AuthenticatedSidebarPageLayoutNavbarPageLayoutRouteWithChildren =
  AuthenticatedSidebarPageLayoutNavbarPageLayoutRoute._addFileChildren(
    AuthenticatedSidebarPageLayoutNavbarPageLayoutRouteChildren,
  )

interface AuthenticatedSidebarPageLayoutRouteChildren {
  AuthenticatedSidebarPageLayoutNavbarPageLayoutRoute: typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutRouteWithChildren
}

const AuthenticatedSidebarPageLayoutRouteChildren: AuthenticatedSidebarPageLayoutRouteChildren =
  {
    AuthenticatedSidebarPageLayoutNavbarPageLayoutRoute:
      AuthenticatedSidebarPageLayoutNavbarPageLayoutRouteWithChildren,
  }

const AuthenticatedSidebarPageLayoutRouteWithChildren =
  AuthenticatedSidebarPageLayoutRoute._addFileChildren(
    AuthenticatedSidebarPageLayoutRouteChildren,
  )

interface AuthenticatedRouteChildren {
  AuthenticatedSidebarPageLayoutRoute: typeof AuthenticatedSidebarPageLayoutRouteWithChildren
}

const AuthenticatedRouteChildren: AuthenticatedRouteChildren = {
  AuthenticatedSidebarPageLayoutRoute:
    AuthenticatedSidebarPageLayoutRouteWithChildren,
}

const AuthenticatedRouteWithChildren = AuthenticatedRoute._addFileChildren(
  AuthenticatedRouteChildren,
)

export interface FileRoutesByFullPath {
  '/': typeof IndexRoute
  '': typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutRouteWithChildren
  '/auth': typeof AuthRoute
  '/debug': typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutDebugRoute
  '/home': typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutHomeRoute
  '/test': typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutTestRoute
  '/nutrition/diary': typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionDiaryRoute
  '/nutrition/recipes': typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionRecipesRoute
}

export interface FileRoutesByTo {
  '/': typeof IndexRoute
  '': typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutRouteWithChildren
  '/auth': typeof AuthRoute
  '/debug': typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutDebugRoute
  '/home': typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutHomeRoute
  '/test': typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutTestRoute
  '/nutrition/diary': typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionDiaryRoute
  '/nutrition/recipes': typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionRecipesRoute
}

export interface FileRoutesById {
  __root__: typeof rootRoute
  '/': typeof IndexRoute
  '/_authenticated': typeof AuthenticatedRouteWithChildren
  '/auth': typeof AuthRoute
  '/_authenticated/_sidebarPageLayout': typeof AuthenticatedSidebarPageLayoutRouteWithChildren
  '/_authenticated/_sidebarPageLayout/_navbarPageLayout': typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutRouteWithChildren
  '/_authenticated/_sidebarPageLayout/_navbarPageLayout/debug': typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutDebugRoute
  '/_authenticated/_sidebarPageLayout/_navbarPageLayout/home': typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutHomeRoute
  '/_authenticated/_sidebarPageLayout/_navbarPageLayout/test': typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutTestRoute
  '/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/diary': typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionDiaryRoute
  '/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/recipes': typeof AuthenticatedSidebarPageLayoutNavbarPageLayoutNutritionRecipesRoute
}

export interface FileRouteTypes {
  fileRoutesByFullPath: FileRoutesByFullPath
  fullPaths:
    | '/'
    | ''
    | '/auth'
    | '/debug'
    | '/home'
    | '/test'
    | '/nutrition/diary'
    | '/nutrition/recipes'
  fileRoutesByTo: FileRoutesByTo
  to:
    | '/'
    | ''
    | '/auth'
    | '/debug'
    | '/home'
    | '/test'
    | '/nutrition/diary'
    | '/nutrition/recipes'
  id:
    | '__root__'
    | '/'
    | '/_authenticated'
    | '/auth'
    | '/_authenticated/_sidebarPageLayout'
    | '/_authenticated/_sidebarPageLayout/_navbarPageLayout'
    | '/_authenticated/_sidebarPageLayout/_navbarPageLayout/debug'
    | '/_authenticated/_sidebarPageLayout/_navbarPageLayout/home'
    | '/_authenticated/_sidebarPageLayout/_navbarPageLayout/test'
    | '/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/diary'
    | '/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/recipes'
  fileRoutesById: FileRoutesById
}

export interface RootRouteChildren {
  IndexRoute: typeof IndexRoute
  AuthenticatedRoute: typeof AuthenticatedRouteWithChildren
  AuthRoute: typeof AuthRoute
}

const rootRouteChildren: RootRouteChildren = {
  IndexRoute: IndexRoute,
  AuthenticatedRoute: AuthenticatedRouteWithChildren,
  AuthRoute: AuthRoute,
}

export const routeTree = rootRoute
  ._addFileChildren(rootRouteChildren)
  ._addFileTypes<FileRouteTypes>()

/* ROUTE_MANIFEST_START
{
  "routes": {
    "__root__": {
      "filePath": "__root.tsx",
      "children": [
        "/",
        "/_authenticated",
        "/auth"
      ]
    },
    "/": {
      "filePath": "index.tsx"
    },
    "/_authenticated": {
      "filePath": "_authenticated.tsx",
      "children": [
        "/_authenticated/_sidebarPageLayout"
      ]
    },
    "/auth": {
      "filePath": "auth.tsx"
    },
    "/_authenticated/_sidebarPageLayout": {
      "filePath": "_authenticated/_sidebarPageLayout.tsx",
      "parent": "/_authenticated",
      "children": [
        "/_authenticated/_sidebarPageLayout/_navbarPageLayout"
      ]
    },
    "/_authenticated/_sidebarPageLayout/_navbarPageLayout": {
      "filePath": "_authenticated/_sidebarPageLayout/_navbarPageLayout.tsx",
      "parent": "/_authenticated/_sidebarPageLayout",
      "children": [
        "/_authenticated/_sidebarPageLayout/_navbarPageLayout/debug",
        "/_authenticated/_sidebarPageLayout/_navbarPageLayout/home",
        "/_authenticated/_sidebarPageLayout/_navbarPageLayout/test",
        "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/diary",
        "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/recipes"
      ]
    },
    "/_authenticated/_sidebarPageLayout/_navbarPageLayout/debug": {
      "filePath": "_authenticated/_sidebarPageLayout/_navbarPageLayout/debug.tsx",
      "parent": "/_authenticated/_sidebarPageLayout/_navbarPageLayout"
    },
    "/_authenticated/_sidebarPageLayout/_navbarPageLayout/home": {
      "filePath": "_authenticated/_sidebarPageLayout/_navbarPageLayout/home.tsx",
      "parent": "/_authenticated/_sidebarPageLayout/_navbarPageLayout"
    },
    "/_authenticated/_sidebarPageLayout/_navbarPageLayout/test": {
      "filePath": "_authenticated/_sidebarPageLayout/_navbarPageLayout/test.tsx",
      "parent": "/_authenticated/_sidebarPageLayout/_navbarPageLayout"
    },
    "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/diary": {
      "filePath": "_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/diary.tsx",
      "parent": "/_authenticated/_sidebarPageLayout/_navbarPageLayout"
    },
    "/_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/recipes": {
      "filePath": "_authenticated/_sidebarPageLayout/_navbarPageLayout/nutrition/recipes.tsx",
      "parent": "/_authenticated/_sidebarPageLayout/_navbarPageLayout"
    }
  }
}
ROUTE_MANIFEST_END */