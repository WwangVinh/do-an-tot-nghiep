import { Outlet, Link, useLocation, useNavigate } from "react-router";
import { 
  LayoutDashboard, 
  Car, 
  Users, 
  LogOut, 
  Menu,
  X,
  UserCircle,
  Home
} from "lucide-react";
import { useState, useEffect } from "react";
import { AuthService } from "../../services/authService";

export function AdminLayout() {
  const [sidebarOpen, setSidebarOpen] = useState(true);
  const [userInfo, setUserInfo] = useState<any>(null);
  const location = useLocation();
  const navigate = useNavigate();

  useEffect(() => {
    // Check if user is authenticated
    if (!AuthService.isAuthenticated()) {
      navigate("/");
      return;
    }
    
    setUserInfo(AuthService.getUserInfo());
  }, [navigate]);

  const handleLogout = () => {
    AuthService.logout();
    navigate("/");
  };

  const menuItems = [
    { path: "/admin", icon: LayoutDashboard, label: "Dashboard" },
    { path: "/admin/cars", icon: Car, label: "Quản lý xe" },
    { path: "/admin/users", icon: Users, label: "Quản lý người dùng" },
  ];

  return (
    <div className="flex h-screen bg-gray-100">
      {/* Sidebar */}
      <aside
        className={`${
          sidebarOpen ? "w-64" : "w-20"
        } bg-gray-900 text-white transition-all duration-300 ease-in-out flex flex-col`}
      >
        {/* Logo */}
        <div className="flex items-center justify-between p-4 border-b border-gray-700">
          {sidebarOpen && (
            <div className="flex items-center gap-2">
              <div className="rounded bg-red-600 px-3 py-1">
                <span className="text-lg font-bold">AutoLux</span>
              </div>
            </div>
          )}
          <button
            onClick={() => setSidebarOpen(!sidebarOpen)}
            className="p-2 rounded hover:bg-gray-800 transition-colors"
          >
            {sidebarOpen ? <X className="h-5 w-5" /> : <Menu className="h-5 w-5" />}
          </button>
        </div>

        {/* Navigation */}
        <nav className="flex-1 p-4">
          <ul className="space-y-2">
            {menuItems.map((item) => {
              const Icon = item.icon;
              const isActive = location.pathname === item.path;
              
              return (
                <li key={item.path}>
                  <Link
                    to={item.path}
                    className={`flex items-center gap-3 px-4 py-3 rounded transition-colors ${
                      isActive
                        ? "bg-red-600 text-white"
                        : "text-gray-300 hover:bg-gray-800"
                    }`}
                  >
                    <Icon className="h-5 w-5 flex-shrink-0" />
                    {sidebarOpen && <span>{item.label}</span>}
                  </Link>
                </li>
              );
            })}
          </ul>
        </nav>

        {/* User info */}
        <div className="border-t border-gray-700 p-4">
          <div className="flex items-center gap-3">
            <div className="flex h-10 w-10 items-center justify-center rounded-full bg-red-600 font-bold flex-shrink-0">
              {userInfo?.fullName?.[0] || "A"}
            </div>
            {sidebarOpen && (
              <div className="flex-1 min-w-0">
                <p className="text-sm font-semibold truncate">
                  {userInfo?.fullName || "Admin"}
                </p>
                <p className="text-xs text-gray-400 truncate">
                  {userInfo?.email || ""}
                </p>
              </div>
            )}
          </div>
          <a
            href="/"
            className="mt-3 flex items-center gap-3 px-4 py-2 rounded w-full text-gray-300 hover:bg-gray-800 transition-colors"
          >
            <Home className="h-5 w-5 flex-shrink-0" />
            {sidebarOpen && <span>Về trang chủ</span>}
          </a>
          <button
            onClick={handleLogout}
            className="mt-2 flex items-center gap-3 px-4 py-2 rounded w-full text-gray-300 hover:bg-gray-800 transition-colors"
          >
            <LogOut className="h-5 w-5 flex-shrink-0" />
            {sidebarOpen && <span>Đăng xuất</span>}
          </button>
        </div>
      </aside>

      {/* Main content */}
      <main className="flex-1 overflow-auto">
        <div className="p-8">
          <Outlet />
        </div>
      </main>
    </div>
  );
}