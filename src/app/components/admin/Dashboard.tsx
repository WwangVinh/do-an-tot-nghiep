import { Car, Users, DollarSign, TrendingUp } from "lucide-react";
import { useEffect, useState } from "react";
import { CarService } from "../../services/carService";

export function Dashboard() {
  const [stats, setStats] = useState({
    totalCars: 0,
    totalUsers: 0,
    totalRevenue: 0,
    newCars: 0,
  });

  useEffect(() => {
    loadStats();
  }, []);

  const loadStats = async () => {
    // Load car statistics
    const carData = await CarService.getCarList({ pageSize: 1000 });
    
    setStats({
      totalCars: carData.totalCount,
      totalUsers: 156, // Mock data
      totalRevenue: 15600000000, // Mock data
      newCars: carData.data.filter(c => c.status === 0).length,
    });
  };

  const statCards = [
    {
      title: "Tổng số xe",
      value: stats.totalCars,
      icon: Car,
      color: "bg-blue-500",
      textColor: "text-blue-500",
    },
    {
      title: "Người dùng",
      value: stats.totalUsers,
      icon: Users,
      color: "bg-green-500",
      textColor: "text-green-500",
    },
    {
      title: "Doanh thu",
      value: `${(stats.totalRevenue / 1000000000).toFixed(1)}B VNĐ`,
      icon: DollarSign,
      color: "bg-yellow-500",
      textColor: "text-yellow-500",
    },
    {
      title: "Xe mới",
      value: stats.newCars,
      icon: TrendingUp,
      color: "bg-red-500",
      textColor: "text-red-500",
    },
  ];

  return (
    <div>
      <h1 className="text-3xl font-bold text-gray-900 mb-8">Dashboard</h1>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        {statCards.map((stat) => {
          const Icon = stat.icon;
          return (
            <div
              key={stat.title}
              className="bg-white rounded-lg shadow p-6 hover:shadow-lg transition-shadow"
            >
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-gray-500 text-sm mb-1">{stat.title}</p>
                  <p className="text-2xl font-bold text-gray-900">
                    {stat.value}
                  </p>
                </div>
                <div className={`${stat.color} p-3 rounded-lg`}>
                  <Icon className="h-6 w-6 text-white" />
                </div>
              </div>
            </div>
          );
        })}
      </div>

      {/* Quick Actions */}
      <div className="bg-white rounded-lg shadow p-6">
        <h2 className="text-xl font-bold text-gray-900 mb-4">Hành động nhanh</h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <a
            href="/admin/cars/new"
            className="flex items-center gap-3 p-4 border-2 border-dashed border-gray-300 rounded-lg hover:border-red-600 hover:bg-red-50 transition-colors"
          >
            <Car className="h-6 w-6 text-red-600" />
            <div>
              <p className="font-semibold text-gray-900">Thêm xe mới</p>
              <p className="text-sm text-gray-500">Tạo mục xe mới</p>
            </div>
          </a>
          <a
            href="/admin/cars"
            className="flex items-center gap-3 p-4 border-2 border-dashed border-gray-300 rounded-lg hover:border-blue-600 hover:bg-blue-50 transition-colors"
          >
            <Car className="h-6 w-6 text-blue-600" />
            <div>
              <p className="font-semibold text-gray-900">Quản lý xe</p>
              <p className="text-sm text-gray-500">Xem tất cả xe</p>
            </div>
          </a>
          <a
            href="/admin/users"
            className="flex items-center gap-3 p-4 border-2 border-dashed border-gray-300 rounded-lg hover:border-green-600 hover:bg-green-50 transition-colors"
          >
            <Users className="h-6 w-6 text-green-600" />
            <div>
              <p className="font-semibold text-gray-900">Quản lý người dùng</p>
              <p className="text-sm text-gray-500">Xem người dùng</p>
            </div>
          </a>
        </div>
      </div>
    </div>
  );
}