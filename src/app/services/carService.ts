// API base URL - sử dụng cùng URL với authService
const API_BASE_URL = "https://draconially-tintometric-stevie.ngrok-free.dev";

export interface Car {
  id?: number;
  name: string;
  brand: string;
  color: string;
  price: number;
  status: number; // 0, 1, 2, 3
  transmission: string;
  bodyStyle: string;
  image?: string;
  description?: string;
  year?: number;
  mileage?: number;
}

export interface CarListParams {
  search?: string;
  brand?: string;
  color?: string;
  minPrice?: number;
  maxPrice?: number;
  status?: number;
  transmission?: string;
  bodyStyle?: string;
  page?: number;
  pageSize?: number;
}

export interface CarListResponse {
  data: Car[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export class CarService {
  /**
   * Lấy danh sách xe với phân trang và lọc
   */
  static async getCarList(params: CarListParams = {}): Promise<CarListResponse> {
    try {
      const queryParams = new URLSearchParams();
      
      Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== "") {
          queryParams.append(key, value.toString());
        }
      });

      const response = await fetch(
        `${API_BASE_URL}/api/Cars?${queryParams.toString()}`,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${localStorage.getItem("accessToken")}`,
            "ngrok-skip-browser-warning": "true",
          },
        }
      );

      if (!response.ok) {
        console.error("API Response not OK:", response.status, response.statusText);
        throw new Error(`Failed to fetch cars: ${response.status}`);
      }

      const contentType = response.headers.get("content-type");
      if (!contentType || !contentType.includes("application/json")) {
        console.error("Response is not JSON:", contentType);
        const text = await response.text();
        console.error("Response body:", text.substring(0, 500));
        throw new Error("Server returned non-JSON response");
      }

      const result = await response.json();
      
      return {
        data: result.data || result || [],
        totalCount: result.totalCount || result.length || 0,
        page: params.page || 1,
        pageSize: params.pageSize || 12,
      };
    } catch (error) {
      console.error("Get car list error:", error);
      return {
        data: [],
        totalCount: 0,
        page: 1,
        pageSize: 12,
      };
    }
  }

  /**
   * Lấy chi tiết một xe
   */
  static async getCarById(id: number): Promise<Car | null> {
    try {
      const response = await fetch(
        `${API_BASE_URL}/api/Cars/${id}`,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${localStorage.getItem("accessToken")}`,
            "ngrok-skip-browser-warning": "true",
          },
        }
      );

      if (!response.ok) {
        throw new Error("Failed to fetch car");
      }

      return await response.json();
    } catch (error) {
      console.error("Get car by ID error:", error);
      return null;
    }
  }

  /**
   * Tạo xe mới
   */
  static async createCar(car: Car): Promise<{ success: boolean; message: string; data?: Car }> {
    try {
      const response = await fetch(
        `${API_BASE_URL}/api/Cars`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${localStorage.getItem("accessToken")}`,
            "ngrok-skip-browser-warning": "true",
          },
          body: JSON.stringify(car),
        }
      );

      const result = await response.json();

      if (response.ok) {
        return {
          success: true,
          message: "Thêm xe thành công!",
          data: result,
        };
      } else {
        return {
          success: false,
          message: result.message || "Thêm xe thất bại!",
        };
      }
    } catch (error) {
      console.error("Create car error:", error);
      return {
        success: false,
        message: "Lỗi kết nối đến server!",
      };
    }
  }

  /**
   * Cập nhật thông tin xe
   */
  static async updateCar(id: number, car: Car): Promise<{ success: boolean; message: string; data?: Car }> {
    try {
      const response = await fetch(
        `${API_BASE_URL}/api/Cars/${id}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${localStorage.getItem("accessToken")}`,
            "ngrok-skip-browser-warning": "true",
          },
          body: JSON.stringify(car),
        }
      );

      const result = await response.json();

      if (response.ok) {
        return {
          success: true,
          message: "Cập nhật xe thành công!",
          data: result,
        };
      } else {
        return {
          success: false,
          message: result.message || "Cập nhật xe thất bại!",
        };
      }
    } catch (error) {
      console.error("Update car error:", error);
      return {
        success: false,
        message: "Lỗi kết nối đến server!",
      };
    }
  }

  /**
   * Xóa xe
   */
  static async deleteCar(id: number): Promise<{ success: boolean; message: string }> {
    try {
      const response = await fetch(
        `${API_BASE_URL}/api/Cars/${id}`,
        {
          method: "DELETE",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${localStorage.getItem("accessToken")}`,
            "ngrok-skip-browser-warning": "true",
          },
        }
      );

      if (response.ok) {
        return {
          success: true,
          message: "Xóa xe thành công!",
        };
      } else {
        const result = await response.json();
        return {
          success: false,
          message: result.message || "Xóa xe thất bại!",
        };
      }
    } catch (error) {
      console.error("Delete car error:", error);
      return {
        success: false,
        message: "Lỗi kết nối đến server!",
      };
    }
  }

  /**
   * Lấy danh sách status
   */
  static getStatusList() {
    return [
      { value: 0, label: "Mới" },
      { value: 1, label: "Đã qua sử dụng" },
      { value: 2, label: "Đang bảo dưỡng" },
      { value: 3, label: "Đã bán" },
    ];
  }

  /**
   * Lấy status label
   */
  static getStatusLabel(status: number): string {
    const statusList = this.getStatusList();
    return statusList.find(s => s.value === status)?.label || "Không xác định";
  }
}