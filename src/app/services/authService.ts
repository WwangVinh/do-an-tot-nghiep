// API base URL - thay đổi URL ngrok của bạn tại đây
const API_BASE_URL =
  " https://draconially-tintometric-stevie.ngrok-free.dev";

export interface RegisterRequest {
  username: string;
  password: string;
  email: string;
  fullName: string;
  phone: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface AuthResponse {
  success: boolean;
  message: string;
  access?: string; // JWT token
  data?: any;
}

export class AuthService {
  /**
   * Đăng ký tài khoản mới
   */
  static async register(
    data: RegisterRequest,
  ): Promise<AuthResponse> {
    try {
      const response = await fetch(
        `${API_BASE_URL}/api/Auth/register`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(data),
        },
      );

      const result = await response.json();

      if (response.ok) {
        return {
          success: true,
          message: "Đăng ký thành công!",
          data: result,
        };
      } else {
        return {
          success: false,
          message:
            result.message ||
            "Đăng ký thất bại. Vui lòng thử lại.",
        };
      }
    } catch (error) {
      console.error("Register error:", error);
      return {
        success: false,
        message:
          "Lỗi kết nối đến server. Vui lòng kiểm tra lại.",
      };
    }
  }

  /**
   * Đăng nhập
   */
  static async login(
    data: LoginRequest,
  ): Promise<AuthResponse> {
    try {
      const response = await fetch(
        `${API_BASE_URL}/api/Auth/login`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(data),
        },
      );

      const result = await response.json();
      
      console.log("API Response:", {
        ok: response.ok,
        status: response.status,
        result: result
      });

      // API trả về 'token' hoặc 'access'
      const token = result.token || result.access;

      if (response.ok && token) {
        // Lưu token vào localStorage
        localStorage.setItem("accessToken", token);
        localStorage.setItem(
          "userInfo",
          JSON.stringify(result),
        );

        return {
          success: true,
          message: result.message || "Đăng nhập thành công!",
          access: token,
          data: result,
        };
      } else {
        return {
          success: false,
          message:
            result.message ||
            "Sai tên đăng nhập hoặc mật khẩu.",
        };
      }
    } catch (error) {
      console.error("Login error:", error);
      return {
        success: false,
        message:
          "Lỗi kết nối đến server. Vui lòng kiểm tra lại.",
      };
    }
  }

  /**
   * Đăng xuất
   */
  static logout(): void {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("userInfo");
  }

  /**
   * Kiểm tra đã đăng nhập chưa
   */
  static isAuthenticated(): boolean {
    return !!localStorage.getItem("accessToken");
  }

  /**
   * Lấy access token
   */
  static getAccessToken(): string | null {
    return localStorage.getItem("accessToken");
  }

  /**
   * Lấy thông tin user
   */
  static getUserInfo(): any {
    const userInfo = localStorage.getItem("userInfo");
    return userInfo ? JSON.parse(userInfo) : null;
  }
}