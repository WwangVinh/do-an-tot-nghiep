import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router";
import { CarService, Car } from "../../services/carService";
import { Button } from "../ui/button";
import { ArrowLeft, Save } from "lucide-react";
import { toast } from "sonner";

export function CarForm() {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEdit = !!id;

  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState<Car>({
    name: "",
    brand: "",
    color: "",
    price: 0,
    status: 0,
    transmission: "",
    bodyStyle: "",
    image: "",
    description: "",
    year: new Date().getFullYear(),
    mileage: 0,
  });

  useEffect(() => {
    if (isEdit) {
      loadCar();
    }
  }, [id]);

  const loadCar = async () => {
    if (!id) return;
    
    setLoading(true);
    const car = await CarService.getCarById(Number(id));
    if (car) {
      setFormData(car);
    } else {
      toast.error("Không tìm thấy xe");
      navigate("/admin/cars");
    }
    setLoading(false);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Validation
    if (!formData.name || !formData.brand || !formData.price) {
      toast.error("Vui lòng điền đầy đủ thông tin bắt buộc");
      return;
    }

    setLoading(true);

    let result;
    if (isEdit && id) {
      result = await CarService.updateCar(Number(id), formData);
    } else {
      result = await CarService.createCar(formData);
    }

    setLoading(false);

    if (result.success) {
      toast.success(result.message);
      navigate("/admin/cars");
    } else {
      toast.error(result.message);
    }
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
  ) => {
    const { name, value, type } = e.target;
    
    setFormData({
      ...formData,
      [name]: type === "number" ? Number(value) : value,
    });
  };

  if (loading && isEdit) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="inline-block h-8 w-8 animate-spin rounded-full border-4 border-solid border-red-600 border-r-transparent"></div>
      </div>
    );
  }

  return (
    <div>
      {/* Header */}
      <div className="flex items-center gap-4 mb-8">
        <button
          onClick={() => navigate("/admin/cars")}
          className="p-2 hover:bg-gray-100 rounded-lg transition-colors"
        >
          <ArrowLeft className="h-6 w-6" />
        </button>
        <h1 className="text-3xl font-bold text-gray-900">
          {isEdit ? "Chỉnh sửa xe" : "Thêm xe mới"}
        </h1>
      </div>

      {/* Form */}
      <form onSubmit={handleSubmit} className="bg-white rounded-lg shadow p-6">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {/* Tên xe */}
          <div className="md:col-span-2">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Tên xe <span className="text-red-600">*</span>
            </label>
            <input
              type="text"
              name="name"
              value={formData.name}
              onChange={handleChange}
              required
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-red-600"
              placeholder="Ví dụ: Honda Civic 2024"
            />
          </div>

          {/* Thương hiệu */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Thương hiệu <span className="text-red-600">*</span>
            </label>
            <input
              type="text"
              name="brand"
              value={formData.brand}
              onChange={handleChange}
              required
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-red-600"
              placeholder="Ví dụ: Honda"
            />
          </div>

          {/* Màu sắc */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Màu sắc
            </label>
            <input
              type="text"
              name="color"
              value={formData.color}
              onChange={handleChange}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-red-600"
              placeholder="Ví dụ: Đen"
            />
          </div>

          {/* Giá */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Giá (VNĐ) <span className="text-red-600">*</span>
            </label>
            <input
              type="number"
              name="price"
              value={formData.price}
              onChange={handleChange}
              required
              min="0"
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-red-600"
              placeholder="Ví dụ: 750000000"
            />
          </div>

          {/* Trạng thái */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Trạng thái
            </label>
            <select
              name="status"
              value={formData.status}
              onChange={handleChange}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-red-600"
            >
              {CarService.getStatusList().map((status) => (
                <option key={`form-status-${status.value}`} value={status.value}>
                  {status.label}
                </option>
              ))}
            </select>
          </div>

          {/* Hộp số */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Hộp số
            </label>
            <select
              name="transmission"
              value={formData.transmission}
              onChange={handleChange}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-red-600"
            >
              <option value="">Chọn hộp số</option>
              <option value="Tự động">Tự động</option>
              <option value="Số sàn">Số sàn</option>
              <option value="Bán tự động">Bán tự động</option>
            </select>
          </div>

          {/* Kiểu dáng */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Kiểu dáng
            </label>
            <select
              name="bodyStyle"
              value={formData.bodyStyle}
              onChange={handleChange}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-red-600"
            >
              <option value="">Chọn kiểu dáng</option>
              <option value="Sedan">Sedan</option>
              <option value="SUV">SUV</option>
              <option value="Hatchback">Hatchback</option>
              <option value="MPV">MPV</option>
              <option value="Coupe">Coupe</option>
              <option value="Pickup">Pickup</option>
            </select>
          </div>

          {/* Năm sản xuất */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Năm sản xuất
            </label>
            <input
              type="number"
              name="year"
              value={formData.year}
              onChange={handleChange}
              min="1900"
              max={new Date().getFullYear() + 1}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-red-600"
            />
          </div>

          {/* Số km đã đi */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Số km đã đi
            </label>
            <input
              type="number"
              name="mileage"
              value={formData.mileage}
              onChange={handleChange}
              min="0"
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-red-600"
              placeholder="Ví dụ: 15000"
            />
          </div>

          {/* URL hình ảnh */}
          <div className="md:col-span-2">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              URL hình ảnh
            </label>
            <input
              type="text"
              name="image"
              value={formData.image}
              onChange={handleChange}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-red-600"
              placeholder="https://example.com/car-image.jpg"
            />
            {formData.image && (
              <img
                src={formData.image}
                alt="Preview"
                className="mt-2 h-32 object-cover rounded-lg"
                onError={(e) => {
                  e.currentTarget.style.display = "none";
                }}
              />
            )}
          </div>

          {/* Mô tả */}
          <div className="md:col-span-2">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Mô tả
            </label>
            <textarea
              name="description"
              value={formData.description}
              onChange={handleChange}
              rows={4}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-red-600"
              placeholder="Nhập mô tả chi tiết về xe..."
            />
          </div>
        </div>

        {/* Actions */}
        <div className="flex justify-end gap-4 mt-6 pt-6 border-t">
          <Button
            type="button"
            variant="outline"
            onClick={() => navigate("/admin/cars")}
          >
            Hủy
          </Button>
          <Button
            type="submit"
            disabled={loading}
            className="bg-red-600 hover:bg-red-700 text-white"
          >
            {loading ? (
              <>
                <div className="inline-block h-4 w-4 animate-spin rounded-full border-2 border-solid border-white border-r-transparent mr-2"></div>
                Đang lưu...
              </>
            ) : (
              <>
                <Save className="h-4 w-4 mr-2" />
                {isEdit ? "Cập nhật" : "Thêm mới"}
              </>
            )}
          </Button>
        </div>
      </form>
    </div>
  );
}