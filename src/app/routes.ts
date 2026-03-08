import { createBrowserRouter } from "react-router";
import { AdminLayout } from "./components/admin/AdminLayout";
import { Dashboard } from "./components/admin/Dashboard";
import { CarList } from "./components/admin/CarList";
import { CarForm } from "./components/admin/CarForm";
import { UserList } from "./components/admin/UserList";

export const router = createBrowserRouter([
  {
    path: "/admin",
    Component: AdminLayout,
    children: [
      {
        index: true,
        Component: Dashboard,
      },
      {
        path: "cars",
        Component: CarList,
      },
      {
        path: "cars/new",
        Component: CarForm,
      },
      {
        path: "cars/edit/:id",
        Component: CarForm,
      },
      {
        path: "users",
        Component: UserList,
      },
    ],
  },
]);
