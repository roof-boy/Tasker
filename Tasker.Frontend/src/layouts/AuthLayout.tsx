import { Outlet } from "react-router";

export default function AuthLayout() {
  return (
    <div className="bg-gray-100 h-screen w-screen flex items-center justify-center">
      <Outlet />
    </div>
  );
}
