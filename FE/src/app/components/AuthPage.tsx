import { useState, useEffect } from "react";
import { Car, Eye, EyeOff, Mail, Lock, User, Phone, ArrowLeft, AlertCircle, CheckCircle2 } from "lucide-react";
import { Button } from "./ui/button";
import { Input } from "./ui/input";
import { Label } from "./ui/label";
import { Checkbox } from "./ui/checkbox";
import { ImageWithFallback } from "./figma/ImageWithFallback";
import { AuthService } from "../services/authService";

const BACKGROUND_IMAGES = [
  "https://images.unsplash.com/photo-1763165524637-9067debdc80b?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxsdXh1cnklMjBjYXIlMjBkZWFsZXJzaGlwJTIwc2hvd3Jvb218ZW58MXx8fHwxNzcyNjI2MTQ5fDA&ixlib=rb-4.1.0&q=80&w=1080",
  "https://images.unsplash.com/photo-1647340764627-11713b9d0f65?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxsdXh1cnklMjBzcG9ydHMlMjBjYXIlMjBzaG93cm9vbXxlbnwxfHx8fDE3NzI2MTYxMTV8MA&ixlib=rb-4.1.0&q=80&w=1080",
  "https://images.unsplash.com/photo-1771284848877-f37b8aa40419?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxwcmVtaXVtJTIwY2FyJTIwZGVhbGVyc2hpcCUyMGludGVyaW9yfGVufDF8fHx8MTc3MjY0MDc5Nnww&ixlib=rb-4.1.0&q=80&w=1080",
  "https://images.unsplash.com/photo-1772548570186-06a119f71784?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxleG90aWMlMjBzdXBlcmNhciUyMGRpc3BsYXl8ZW58MXx8fHwxNzcyNjQwNzk2fDA&ixlib=rb-4.1.0&q=80&w=1080",
  "https://images.unsplash.com/photo-1567214871203-640282b6bdd3?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxoaWdoJTIwZW5kJTIwYXV0b21vYmlsZSUyMGNvbGxlY3Rpb258ZW58MXx8fHwxNzcyNjQwNzk3fDA&ixlib=rb-4.1.0&q=80&w=1080",
  "https://images.unsplash.com/photo-1769641241120-f0dd8669b12d?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxsdXh1cnklMjB2ZWhpY2xlJTIwZ2FyYWdlfGVufDF8fHx8MTc3MjY0MDc5N3ww&ixlib=rb-4.1.0&q=80&w=1080",
];

interface AuthPageProps {
  onBackToHome?: () => void;
}

export function AuthPage({ onBackToHome }: AuthPageProps = {}) {
  const [isLogin, setIsLogin] = useState(true);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [currentImageIndex, setCurrentImageIndex] = useState(0);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState<{ type: "success" | "error"; text: string } | null>(null);
  
  // Form data
  const [formData, setFormData] = useState({
    username: "",
    email: "",
    password: "",
    confirmPassword: "",
    fullName: "",
    phone: "",
  });

  useEffect(() => {
    const interval = setInterval(() => {
      setCurrentImageIndex((prevIndex) => 
        (prevIndex + 1) % BACKGROUND_IMAGES.length
      );
    }, 2000); // Đổi ảnh mỗi 2 giây

    return () => clearInterval(interval);
  }, []);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { id, value } = e.target;
    // Map "name" field to "fullName" in formData
    const fieldName = id === "name" ? "fullName" : id;
    setFormData((prev) => ({ ...prev, [fieldName]: value }));
    // Clear message when user types
    if (message) setMessage(null);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setMessage(null);

    try {
      if (isLogin) {
        // Đăng nhập với username
        const result = await AuthService.login({
          username: formData.username,
          password: formData.password,
        });

        console.log("Login result:", result);

        if (result.success) {
          setMessage({ type: "success", text: result.message });
          // Trigger a custom event to notify Header component
          window.dispatchEvent(new Event("storage"));
          
          console.log("About to call onBackToHome, callback exists:", !!onBackToHome);
          
          // Chuyển về trang chủ ngay lập tức (chỉ delay 300ms để user thấy thông báo)
          setTimeout(() => {
            console.log("Executing onBackToHome callback");
            if (onBackToHome) {
              onBackToHome();
            } else {
              console.error("onBackToHome is not defined!");
            }
          }, 300);
        } else {
          setMessage({ type: "error", text: result.message });
        }
      } else {
        // Đăng ký
        if (formData.password !== formData.confirmPassword) {
          setMessage({ type: "error", text: "Mật khẩu xác nhận không khớp!" });
          setLoading(false);
          return;
        }

        const result = await AuthService.register({
          username: formData.username,
          email: formData.email,
          password: formData.password,
          fullName: formData.fullName,
          phone: formData.phone,
        });

        if (result.success) {
          setMessage({ type: "success", text: result.message });
          // Chuyển sang form đăng nhập sau 2 giây và giữ lại username
          setTimeout(() => {
            setIsLogin(true);
            setFormData({
              username: formData.username,
              email: "",
              password: "",
              confirmPassword: "",
              fullName: "",
              phone: "",
            });
          }, 2000);
        } else {
          setMessage({ type: "error", text: result.message });
        }
      }
    } catch (error) {
      setMessage({
        type: "error",
        text: "Đã xảy ra lỗi. Vui lòng thử lại sau.",
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen relative overflow-hidden">
      {/* Background Images with Crossfade */}
      <div className="absolute inset-0">
        {BACKGROUND_IMAGES.map((image, index) => (
          <div
            key={index}
            className={`absolute inset-0 transition-opacity duration-1000 ease-in-out ${
              index === currentImageIndex ? "opacity-100" : "opacity-0"
            }`}
          >
            <ImageWithFallback
              src={image}
              alt={`Luxury Car ${index + 1}`}
              className="w-full h-full object-cover"
            />
          </div>
        ))}
        {/* Overlay */}
        <div className="absolute inset-0 bg-gradient-to-br from-slate-900/70 via-slate-900/60 to-blue-900/70 backdrop-blur-[2px]"></div>
      </div>

      {/* Logo and Brand - Top Left */}
      <button 
        onClick={onBackToHome}
        className="absolute top-8 left-8 z-10 flex items-center gap-3 text-white cursor-pointer transition-transform hover:scale-105"
      >
        <div className="bg-white/10 backdrop-blur-md p-3 rounded-full border border-white/20">
          <Car className="size-8" />
        </div>
        <div>
          <h1 className="text-3xl font-bold">AutoLux</h1>
          <p className="text-sm opacity-90">Luxury Car Collection</p>
        </div>
      </button>

      {/* Stats - Top Right */}
      <div className="hidden md:flex absolute top-8 right-8 z-10 gap-6 text-white">
        <div className="text-right">
          <div className="text-2xl font-bold">500+</div>
          <div className="text-xs opacity-75">Xe cao cấp</div>
        </div>
        <div className="text-right">
          <div className="text-2xl font-bold">10K+</div>
          <div className="text-xs opacity-75">Khách hàng</div>
        </div>
        <div className="text-right">
          <div className="text-2xl font-bold">98%</div>
          <div className="text-xs opacity-75">Hài lòng</div>
        </div>
      </div>

      {/* Form Container */}
      <div className="relative z-20 min-h-screen flex items-center justify-center p-4 md:p-8">
        <div className="w-full max-w-md">
          {/* Card chứa form */}
          <div className="bg-white/95 backdrop-blur-xl rounded-3xl shadow-2xl p-8 border border-white/50">
            {/* Header */}
            <div className="text-center mb-8">
              <h2 className="text-3xl font-bold text-slate-900 mb-2">
                {isLogin ? "Chào mừng trở lại" : "Tạo tài khoản mới"}
              </h2>
              <p className="text-slate-600">
                {isLogin
                  ? "Đăng nhập để tiếp tục khám phá"
                  : "Đăng ký để bắt đầu hành trình"}
              </p>
            </div>

            {/* Tabs */}
            <div className="flex gap-2 mb-6 bg-slate-100 p-1 rounded-lg">
              <button
                onClick={() => setIsLogin(true)}
                className={`flex-1 py-2 px-4 rounded-md font-medium transition-all ${
                  isLogin
                    ? "bg-white text-slate-900 shadow-sm"
                    : "text-slate-600 hover:text-slate-900"
                }`}
              >
                Đăng nhập
              </button>
              <button
                onClick={() => setIsLogin(false)}
                className={`flex-1 py-2 px-4 rounded-md font-medium transition-all ${
                  !isLogin
                    ? "bg-white text-slate-900 shadow-sm"
                    : "text-slate-600 hover:text-slate-900"
                }`}
              >
                Đăng ký
              </button>
            </div>

            {/* Message Alert */}
            {message && (
              <div
                className={`mb-4 p-4 rounded-lg flex items-start gap-3 ${
                  message.type === "success"
                    ? "bg-green-50 border border-green-200"
                    : "bg-red-50 border border-red-200"
                }`}
              >
                {message.type === "success" ? (
                  <CheckCircle2 className="size-5 text-green-600 flex-shrink-0 mt-0.5" />
                ) : (
                  <AlertCircle className="size-5 text-red-600 flex-shrink-0 mt-0.5" />
                )}
                <p
                  className={`text-sm ${
                    message.type === "success"
                      ? "text-green-800"
                      : "text-red-800"
                  }`}
                >
                  {message.text}
                </p>
              </div>
            )}

            {/* Form */}
            <form onSubmit={handleSubmit} className="space-y-4">
              {!isLogin && (
                <div className="space-y-2">
                  <Label htmlFor="name" className="text-slate-700">
                    Họ và tên
                  </Label>
                  <div className="relative">
                    <User className="absolute left-3 top-1/2 -translate-y-1/2 size-5 text-slate-400" />
                    <Input
                      id="name"
                      type="text"
                      placeholder="Nguyễn Văn A"
                      className="pl-10 h-12 border-slate-300 focus:border-blue-500"
                      required
                      value={formData.fullName}
                      onChange={handleInputChange}
                    />
                  </div>
                </div>
              )}

              {!isLogin && (
                <div className="space-y-2">
                  <Label htmlFor="username" className="text-slate-700">
                    Tên đăng nhập
                  </Label>
                  <div className="relative">
                    <User className="absolute left-3 top-1/2 -translate-y-1/2 size-5 text-slate-400" />
                    <Input
                      id="username"
                      type="text"
                      placeholder="username123"
                      className="pl-10 h-12 border-slate-300 focus:border-blue-500"
                      required
                      value={formData.username}
                      onChange={handleInputChange}
                    />
                  </div>
                </div>
              )}

              {isLogin ? (
                <div className="space-y-2">
                  <Label htmlFor="username" className="text-slate-700">
                    Tên đăng nhập
                  </Label>
                  <div className="relative">
                    <User className="absolute left-3 top-1/2 -translate-y-1/2 size-5 text-slate-400" />
                    <Input
                      id="username"
                      type="text"
                      placeholder="username123"
                      className="pl-10 h-12 border-slate-300 focus:border-blue-500"
                      required
                      value={formData.username}
                      onChange={handleInputChange}
                    />
                  </div>
                </div>
              ) : (
                <div className="space-y-2">
                  <Label htmlFor="email" className="text-slate-700">
                    Email
                  </Label>
                  <div className="relative">
                    <Mail className="absolute left-3 top-1/2 -translate-y-1/2 size-5 text-slate-400" />
                    <Input
                      id="email"
                      type="email"
                      placeholder="example@email.com"
                      className="pl-10 h-12 border-slate-300 focus:border-blue-500"
                      required
                      value={formData.email}
                      onChange={handleInputChange}
                    />
                  </div>
                </div>
              )}

              {!isLogin && (
                <div className="space-y-2">
                  <Label htmlFor="phone" className="text-slate-700">
                    Số điện thoại
                  </Label>
                  <div className="relative">
                    <Phone className="absolute left-3 top-1/2 -translate-y-1/2 size-5 text-slate-400" />
                    <Input
                      id="phone"
                      type="tel"
                      placeholder="0123456789"
                      className="pl-10 h-12 border-slate-300 focus:border-blue-500"
                      required
                      value={formData.phone}
                      onChange={handleInputChange}
                    />
                  </div>
                </div>
              )}

              <div className="space-y-2">
                <Label htmlFor="password" className="text-slate-700">
                  Mật khẩu
                </Label>
                <div className="relative">
                  <Lock className="absolute left-3 top-1/2 -translate-y-1/2 size-5 text-slate-400" />
                  <Input
                    id="password"
                    type={showPassword ? "text" : "password"}
                    placeholder="••••••••"
                    className="pl-10 pr-10 h-12 border-slate-300 focus:border-blue-500"
                    required
                    value={formData.password}
                    onChange={handleInputChange}
                  />
                  <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400 hover:text-slate-600"
                  >
                    {showPassword ? (
                      <EyeOff className="size-5" />
                    ) : (
                      <Eye className="size-5" />
                    )}
                  </button>
                </div>
              </div>

              {!isLogin && (
                <div className="space-y-2">
                  <Label htmlFor="confirmPassword" className="text-slate-700">
                    Xác nhận mật khẩu
                  </Label>
                  <div className="relative">
                    <Lock className="absolute left-3 top-1/2 -translate-y-1/2 size-5 text-slate-400" />
                    <Input
                      id="confirmPassword"
                      type={showConfirmPassword ? "text" : "password"}
                      placeholder="••••••••"
                      className="pl-10 pr-10 h-12 border-slate-300 focus:border-blue-500"
                      required
                      value={formData.confirmPassword}
                      onChange={handleInputChange}
                    />
                    <button
                      type="button"
                      onClick={() =>
                        setShowConfirmPassword(!showConfirmPassword)
                      }
                      className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400 hover:text-slate-600"
                    >
                      {showConfirmPassword ? (
                        <EyeOff className="size-5" />
                      ) : (
                        <Eye className="size-5" />
                      )}
                    </button>
                  </div>
                </div>
              )}

              {isLogin && (
                <div className="flex items-center justify-between">
                  <div className="flex items-center space-x-2">
                    <Checkbox id="remember" />
                    <label
                      htmlFor="remember"
                      className="text-sm text-slate-600 cursor-pointer"
                    >
                      Ghi nhớ đăng nhập
                    </label>
                  </div>
                  <button
                    type="button"
                    className="text-sm text-blue-600 hover:text-blue-700 hover:underline"
                  >
                    Quên mật khẩu?
                  </button>
                </div>
              )}

              {!isLogin && (
                <div className="flex items-start space-x-2">
                  <Checkbox id="terms" required />
                  <label
                    htmlFor="terms"
                    className="text-sm text-slate-600 cursor-pointer"
                  >
                    Tôi đồng ý với{" "}
                    <button
                      type="button"
                      className="text-blue-600 hover:underline"
                    >
                      Điều khoản dịch vụ
                    </button>{" "}
                    và{" "}
                    <button
                      type="button"
                      className="text-blue-600 hover:underline"
                    >
                      Chính sách bảo mật
                    </button>
                  </label>
                </div>
              )}

              <Button
                type="submit"
                disabled={loading}
                className="w-full h-12 bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800 text-white font-medium text-base shadow-lg shadow-blue-600/30 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {loading ? (
                  <div className="flex items-center justify-center gap-2">
                    <div className="size-5 border-2 border-white/30 border-t-white rounded-full animate-spin"></div>
                    <span>{isLogin ? "Đang đăng nhập..." : "Đang tạo tài khoản..."}</span>
                  </div>
                ) : (
                  <span>{isLogin ? "Đăng nhập" : "Tạo tài khoản"}</span>
                )}
              </Button>
            </form>

            {/* Divider */}
            <div className="relative my-6">
              <div className="absolute inset-0 flex items-center">
                <div className="w-full border-t border-slate-300"></div>
              </div>
              <div className="relative flex justify-center text-sm">
                <span className="px-4 bg-white text-slate-500">
                  Hoặc tiếp tục với
                </span>
              </div>
            </div>

            {/* Social Login */}
            <div className="grid grid-cols-2 gap-3">
              <button
                type="button"
                className="flex items-center justify-center gap-2 py-3 px-4 border border-slate-300 rounded-lg hover:bg-slate-50 transition-colors"
              >
                <svg className="size-5" viewBox="0 0 24 24">
                  <path
                    fill="#4285F4"
                    d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
                  />
                  <path
                    fill="#34A853"
                    d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
                  />
                  <path
                    fill="#FBBC05"
                    d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
                  />
                  <path
                    fill="#EA4335"
                    d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
                  />
                </svg>
                <span className="text-sm font-medium text-slate-700">
                  Google
                </span>
              </button>
              <button
                type="button"
                className="flex items-center justify-center gap-2 py-3 px-4 border border-slate-300 rounded-lg hover:bg-slate-50 transition-colors"
              >
                <svg
                  className="size-5"
                  fill="#1877F2"
                  viewBox="0 0 24 24"
                >
                  <path d="M24 12.073c0-6.627-5.373-12-12-12s-12 5.373-12 12c0 5.99 4.388 10.954 10.125 11.854v-8.385H7.078v-3.47h3.047V9.43c0-3.007 1.792-4.669 4.533-4.669 1.312 0 2.686.235 2.686.235v2.953H15.83c-1.491 0-1.956.925-1.956 1.874v2.25h3.328l-.532 3.47h-2.796v8.385C19.612 23.027 24 18.062 24 12.073z" />
                </svg>
                <span className="text-sm font-medium text-slate-700">
                  Facebook
                </span>
              </button>
            </div>

            {/* Footer */}
            <p className="text-center text-sm text-slate-600 mt-6">
              {isLogin ? "Chưa có tài khoản? " : "Đã có tài khoản? "}
              <button
                onClick={() => setIsLogin(!isLogin)}
                className="text-blue-600 hover:text-blue-700 hover:underline font-medium"
              >
                {isLogin ? "Đăng ký ngay" : "Đăng nhập ngay"}
              </button>
            </p>
            
            {/* Back to Home Button */}
            {onBackToHome && (
              <button
                onClick={onBackToHome}
                className="flex items-center justify-center gap-2 w-full mt-4 py-3 px-4 text-slate-600 hover:text-slate-900 bg-slate-100 hover:bg-slate-200 rounded-lg transition-all font-medium"
              >
                <ArrowLeft className="size-4" />
                Quay lại trang chủ
              </button>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}