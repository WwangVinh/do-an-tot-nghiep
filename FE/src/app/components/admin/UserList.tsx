import { Users } from "lucide-react";

export function UserList() {
  return (
    <div>
      <h1 className="text-3xl font-bold text-gray-900 mb-8">Quản lý người dùng</h1>

      <div className="bg-white rounded-lg shadow p-12 text-center">
        <Users className="h-16 w-16 text-gray-400 mx-auto mb-4" />
        <h2 className="text-xl font-semibold text-gray-700 mb-2">
          Tính năng đang phát triển
        </h2>
        <p className="text-gray-500">
          Trang quản lý người dùng sẽ được cập nhật trong phiên bản tiếp theo
        </p>
      </div>
    </div>
  );
}
