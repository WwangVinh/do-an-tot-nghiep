import { Search, User, Menu, LogOut, Settings, UserCircle, ChevronDown, LayoutDashboard } from "lucide-react";
import { Button } from "./ui/button";
import { useState, useEffect } from "react";
import { AuthService } from "../services/authService";

interface HeaderProps {
  onLoginClick: () => void;
  onLogout?: () => void;
}

export function Header({ onLoginClick, onLogout }: HeaderProps) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [userInfo, setUserInfo] = useState<any>(null);
  const [showDropdown, setShowDropdown] = useState(false);

  useEffect(() => {
    // Check authentication status
    const checkAuth = () => {
      const authenticated = AuthService.isAuthenticated();
      setIsAuthenticated(authenticated);
      if (authenticated) {
        setUserInfo(AuthService.getUserInfo());
      }
    };
    
    checkAuth();
    
    // Listen for storage changes (when user logs in from another tab)
    window.addEventListener("storage", checkAuth);
    
    return () => {
      window.removeEventListener("storage", checkAuth);
    };
  }, []);

  const handleLogout = () => {
    AuthService.logout();
    setIsAuthenticated(false);
    setUserInfo(null);
    setShowDropdown(false);
    if (onLogout) onLogout();
  };

  // Get initials for avatar
  const getInitials = (name: string) => {
    if (!name) return "U";
    const names = name.split(" ");
    if (names.length >= 2) {
      return (names[0][0] + names[names.length - 1][0]).toUpperCase();
    }
    return name.substring(0, 2).toUpperCase();
  };

  return (
    <header className="sticky top-0 z-50 w-full bg-white shadow-sm">
      {/* Top bar */}
      <div className="border-b border-gray-200 bg-gray-50">
        <div className="mx-auto flex max-w-7xl items-center justify-end gap-6 px-4 py-2 text-sm">
          <a
            href="#"
            className="text-gray-700 hover:text-red-600"
          >
            Mỹ Honda
          </a>
          <a
            href="#"
            className="text-gray-700 hover:text-red-600"
          >
            Bản hồng chương nghiệp
          </a>
          <a
            href="#"
            className="text-gray-700 hover:text-red-600"
          >
            Câu hỏi thường gặp
          </a>
          <a
            href="#"
            className="text-gray-700 hover:text-red-600"
          >
            Liên hệ
          </a>
          <a
            href="#"
            className="text-gray-700 hover:text-red-600"
          >
            Tuyển dụng
          </a>
          <a
            href="#"
            className="text-gray-700 hover:text-red-600"
          >
            Thảm gia ý kiến khách hàng
          </a>
          <button className="text-gray-700 hover:text-red-600">
            <Search className="h-4 w-4" />
          </button>
        </div>
      </div>

      {/* Main header */}
      <div className="bg-white">
        <div className="mx-auto flex max-w-7xl items-center justify-between px-4 py-3">
          {/* Logo */}
          <div className="flex items-center gap-2">
            <div className="flex flex-col">
              <div className="flex items-center gap-3">
                <div className="rounded bg-red-600 px-4 py-1.5">
                  <span className="text-2xl font-bold tracking-wider text-white">
                    AutoLux
                  </span>
                </div>
                <div className="flex flex-col text-xs">
                  <span className="font-semibold text-gray-800">
                    How we move you.
                  </span>
                  <span className="text-[10px] text-gray-500">
                    The Power of Dreams
                  </span>
                </div>
              </div>
            </div>
          </div>

          {/* Navigation */}
          <nav className="hidden items-center gap-6 lg:flex">
            <a
              href="#"
              className="text-sm text-gray-700 hover:text-red-600"
            >
              Ô tô
            </a>
            <a
              href="#"
              className="text-sm text-gray-700 hover:text-red-600"
            >
              Xe máy
            </a>
            <a
              href="#"
              className="text-sm text-gray-700 hover:text-red-600"
            >
              Về Honda
            </a>
            <a
              href="#"
              className="text-sm text-gray-700 hover:text-red-600"
            >
              Ấn tượng giao thông
            </a>
            <a
              href="#"
              className="text-sm text-gray-700 hover:text-red-600"
            >
              Dịch vụ sau khi bán
            </a>
            <a
              href="#"
              className="text-sm text-gray-700 hover:text-red-600"
            >
              Phụ tùng và Phụ kiện
            </a>
            <a
              href="#"
              className="text-sm text-gray-700 hover:text-red-600"
            >
              Xe qua sử dụng
            </a>
            <a
              href="#"
              className="text-sm text-gray-700 hover:text-red-600"
            >
              Tin tức
            </a>
          </nav>

          {/* Actions */}
          <div className="flex items-center gap-3">
            {isAuthenticated && userInfo ? (
              <div className="relative">
                <button
                  className="flex items-center gap-2 rounded bg-red-600 px-4 py-2 text-sm text-white hover:bg-red-700 transition-colors"
                  onClick={() => setShowDropdown(!showDropdown)}
                >
                  {/* Avatar */}
                  <div className="flex h-8 w-8 items-center justify-center rounded-full bg-white text-red-600 font-bold text-sm">
                    {getInitials(userInfo.fullName || userInfo.username || "User")}
                  </div>
                  <span className="hidden md:inline font-medium">
                    {userInfo.fullName || userInfo.username || "User"}
                  </span>
                  <ChevronDown className="h-4 w-4" />
                </button>
                
                {/* Dropdown Menu */}
                {showDropdown && (
                  <>
                    {/* Backdrop to close dropdown when clicking outside */}
                    <div
                      className="fixed inset-0 z-10"
                      onClick={() => setShowDropdown(false)}
                    ></div>
                    
                    <div className="absolute right-0 mt-2 w-56 bg-white border border-gray-200 rounded-lg shadow-xl z-20">
                      <div className="py-2">
                        {/* User Info */}
                        <div className="px-4 py-3 border-b border-gray-100">
                          <p className="text-sm font-semibold text-gray-900">
                            {userInfo.fullName || userInfo.username}
                          </p>
                          <p className="text-xs text-gray-500 mt-1">
                            {userInfo.email || ""}
                          </p>
                        </div>
                        
                        {/* Menu Items */}
                        <a
                          href="/admin"
                          className="w-full flex items-center gap-3 px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 transition-colors"
                        >
                          <LayoutDashboard className="h-4 w-4" />
                          Quản lý Admin
                        </a>
                        <button
                          className="w-full flex items-center gap-3 px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 transition-colors"
                          onClick={() => setShowDropdown(false)}
                        >
                          <UserCircle className="h-4 w-4" />
                          Thông tin cá nhân
                        </button>
                        <button
                          className="w-full flex items-center gap-3 px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 transition-colors"
                          onClick={() => setShowDropdown(false)}
                        >
                          <Settings className="h-4 w-4" />
                          Cài đặt
                        </button>
                        
                        <div className="border-t border-gray-100 my-1"></div>
                        
                        <button
                          className="w-full flex items-center gap-3 px-4 py-2 text-sm text-red-600 hover:bg-red-50 transition-colors"
                          onClick={handleLogout}
                        >
                          <LogOut className="h-4 w-4" />
                          Đăng xuất
                        </button>
                      </div>
                    </div>
                  </>
                )}
              </div>
            ) : (
              <Button
                onClick={onLoginClick}
                className="bg-red-600 text-white hover:bg-red-700"
                size="sm"
              >
                <User className="mr-2 h-4 w-4" />
                Đăng nhập
              </Button>
            )}
            <button className="lg:hidden">
              <Menu className="h-6 w-6 text-gray-700" />
            </button>
          </div>
        </div>
      </div>

      {/* Secondary nav */}
      <div className="border-t border-gray-200 bg-red-600">
        <div className="mx-auto flex max-w-7xl items-center justify-between px-4 py-2.5">
          <div className="flex items-center gap-6 overflow-x-auto">
            <a
              href="#"
              className="whitespace-nowrap text-sm text-white hover:text-gray-200"
            >
              Bảng giá
            </a>
            <a
              href="#"
              className="whitespace-nowrap text-sm text-white hover:text-gray-200"
            >
              Bảo hiểm Honda
            </a>
            <a
              href="#"
              className="whitespace-nowrap text-sm text-white hover:text-gray-200"
            >
              Khuyến mại
            </a>
            <a
              href="#"
              className="whitespace-nowrap text-sm text-white hover:text-gray-200"
            >
              Lái thử xe
            </a>
            <a
              href="#"
              className="whitespace-nowrap text-sm text-white hover:text-gray-200"
            >
              Dư lý
            </a>
            <a
              href="#"
              className="whitespace-nowrap text-sm text-white hover:text-gray-200"
            >
              Dịch vụ yêu Bảo hàng
            </a>
            <a
              href="#"
              className="whitespace-nowrap text-sm text-white hover:text-gray-200"
            >
              Phụ tùng và Phụ kiện
            </a>
            <a
              href="#"
              className="whitespace-nowrap text-sm text-white hover:text-gray-200"
            >
              Xe qua sử dụng
            </a>
            <a
              href="#"
              className="whitespace-nowrap text-sm text-white hover:text-gray-200"
            >
              Tin tức
            </a>
          </div>
        </div>
      </div>
    </header>
  );
}