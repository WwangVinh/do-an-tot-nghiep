import { useEffect, useState } from "react";
import { Link } from "react-router";
import { CarService, Car, CarListParams } from "../../services/carService";
import { Plus, Edit, Trash2, Search, Filter } from "lucide-react";
import { Button } from "../ui/button";
import { toast } from "sonner";

export function CarList() {
  const [cars, setCars] = useState<Car[]>([]);
  const [loading, setLoading] = useState(false);
  const [totalCount, setTotalCount] = useState(0);
  const [filters, setFilters] = useState<CarListParams>({
    page: 1,
    pageSize: 12,
  });
  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    loadCars();
  }, [filters]);

  const loadCars = async () => {
    setLoading(true);
    const result = await CarService.getCarList(filters);
    setCars(result.data);
    setTotalCount(result.totalCount);
    setLoading(false);
  };

  const handleSearch = () => {
    setFilters({ ...filters, search: searchTerm, page: 1 });
  };

  const handleDelete = async (id: number) => {
    if (!confirm("Bạn có chắc chắn muốn xóa xe này?")) {
      return;
    }

    const result = await CarService.deleteCar(id);
    if (result.success) {
      toast.success(result.message);
      loadCars();
    } else {
      toast.error(result.message);
    }
  };

  const totalPages = Math.ceil(totalCount / (filters.pageSize || 12));

  return (
    <div>
      {/* Header */}
      <div className="flex items-center justify-between mb-8">
        <h1 className="text-3xl font-bold text-gray-900">Quản lý xe</h1>
        <Link to="/admin/cars/new">
          <Button className="bg-red-600 hover:bg-red-700 text-white">
            <Plus className="h-4 w-4 mr-2" />
            Thêm xe mới
          </Button>
        </Link>
      </div>

      {/* Search & Filter */}
      <div className="bg-white rounded-lg shadow p-4 mb-6">
        <div className="flex gap-4">
          <div className="flex-1">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
              <input
                type="text"
                placeholder="Tìm kiếm theo tên, thương hiệu..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                onKeyPress={(e) => e.key === "Enter" && handleSearch()}
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-red-600"
              />
            </div>
          </div>
          <Button
            onClick={handleSearch}
            className="bg-red-600 hover:bg-red-700 text-white"
          >
            <Search className="h-4 w-4 mr-2" />
            Tìm kiếm
          </Button>
          <Button
            variant="outline"
            onClick={() => {
              setSearchTerm("");
              setFilters({ page: 1, pageSize: 12 });
            }}
          >
            Xóa bộ lọc
          </Button>
        </div>

        {/* Advanced Filters */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mt-4">
          <select
            value={filters.status || ""}
            onChange={(e) =>
              setFilters({
                ...filters,
                status: e.target.value ? Number(e.target.value) : undefined,
                page: 1,
              })
            }
            className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-red-600"
          >
            <option value="" key="status-all">Tất cả trạng thái</option>
            {CarService.getStatusList().map((status) => (
              <option key={`status-${status.value}`} value={status.value}>
                {status.label}
              </option>
            ))}
          </select>

          <input
            type="text"
            placeholder="Thương hiệu"
            value={filters.brand || ""}
            onChange={(e) =>
              setFilters({ ...filters, brand: e.target.value, page: 1 })
            }
            className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-red-600"
          />

          <input
            type="text"
            placeholder="Màu sắc"
            value={filters.color || ""}
            onChange={(e) =>
              setFilters({ ...filters, color: e.target.value, page: 1 })
            }
            className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-red-600"
          />

          <input
            type="text"
            placeholder="Kiểu dáng"
            value={filters.bodyStyle || ""}
            onChange={(e) =>
              setFilters({ ...filters, bodyStyle: e.target.value, page: 1 })
            }
            className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-red-600"
          />
        </div>
      </div>

      {/* Cars Table */}
      <div className="bg-white rounded-lg shadow overflow-hidden">
        {loading ? (
          <div className="p-8 text-center">
            <div className="inline-block h-8 w-8 animate-spin rounded-full border-4 border-solid border-red-600 border-r-transparent"></div>
            <p className="mt-2 text-gray-600">Đang tải...</p>
          </div>
        ) : cars.length === 0 ? (
          <div className="p-8 text-center">
            <p className="text-gray-500">Không tìm thấy xe nào</p>
          </div>
        ) : (
          <>
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      ID
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Tên xe
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Thương hiệu
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Màu sắc
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Giá
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Trạng thái
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Hành động
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {cars.map((car) => (
                    <tr key={car.id} className="hover:bg-gray-50">
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {car.id}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                        {car.name}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        {car.brand}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        {car.color}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {car.price.toLocaleString("vi-VN")} VNĐ
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span
                          className={`px-2 py-1 text-xs rounded-full ${
                            car.status === 0
                              ? "bg-green-100 text-green-800"
                              : car.status === 1
                              ? "bg-blue-100 text-blue-800"
                              : car.status === 2
                              ? "bg-yellow-100 text-yellow-800"
                              : "bg-red-100 text-red-800"
                          }`}
                        >
                          {CarService.getStatusLabel(car.status)}
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm">
                        <div className="flex gap-2">
                          <Link to={`/admin/cars/edit/${car.id}`}>
                            <button className="text-blue-600 hover:text-blue-900">
                              <Edit className="h-4 w-4" />
                            </button>
                          </Link>
                          <button
                            onClick={() => car.id && handleDelete(car.id)}
                            className="text-red-600 hover:text-red-900"
                          >
                            <Trash2 className="h-4 w-4" />
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            {/* Pagination */}
            <div className="bg-gray-50 px-6 py-4 flex items-center justify-between border-t">
              <div className="text-sm text-gray-700">
                Hiển thị{" "}
                <span className="font-medium">
                  {(filters.page! - 1) * filters.pageSize! + 1}
                </span>{" "}
                -{" "}
                <span className="font-medium">
                  {Math.min(filters.page! * filters.pageSize!, totalCount)}
                </span>{" "}
                trong <span className="font-medium">{totalCount}</span> kết quả
              </div>
              <div className="flex gap-2">
                <Button
                  variant="outline"
                  disabled={filters.page === 1}
                  onClick={() =>
                    setFilters({ ...filters, page: filters.page! - 1 })
                  }
                >
                  Trước
                </Button>
                <Button
                  variant="outline"
                  disabled={filters.page === totalPages}
                  onClick={() =>
                    setFilters({ ...filters, page: filters.page! + 1 })
                  }
                >
                  Sau
                </Button>
              </div>
            </div>
          </>
        )}
      </div>
    </div>
  );
}