import { Facebook, Instagram, Youtube, Mail, Phone, MapPin } from "lucide-react";

export function Footer() {
  return (
    <footer className="bg-gray-900 text-gray-300">
      {/* Main Footer */}
      <div className="mx-auto max-w-7xl px-4 py-12">
        <div className="grid grid-cols-1 gap-8 md:grid-cols-2 lg:grid-cols-4">
          {/* Company Info */}
          <div>
            <div className="mb-4 flex items-center gap-3">
              <div className="rounded bg-red-600 px-3 py-1">
                <span className="text-xl font-bold text-white">HONDA</span>
              </div>
            </div>
            <p className="mb-4 text-sm">
              Honda Việt Nam - Đơn vị phân phối chính hãng xe ô tô Honda tại Việt Nam
            </p>
            <div className="flex gap-3">
              <a href="#" className="rounded-full bg-gray-800 p-2 transition hover:bg-red-600">
                <Facebook className="h-5 w-5" />
              </a>
              <a href="#" className="rounded-full bg-gray-800 p-2 transition hover:bg-red-600">
                <Instagram className="h-5 w-5" />
              </a>
              <a href="#" className="rounded-full bg-gray-800 p-2 transition hover:bg-red-600">
                <Youtube className="h-5 w-5" />
              </a>
            </div>
          </div>

          {/* Products */}
          <div>
            <h3 className="mb-4 font-bold text-white">Sản phẩm</h3>
            <ul className="space-y-2 text-sm">
              <li><a href="#" className="hover:text-red-500">Honda City</a></li>
              <li><a href="#" className="hover:text-red-500">Honda BR-V</a></li>
              <li><a href="#" className="hover:text-red-500">Honda HR-V</a></li>
              <li><a href="#" className="hover:text-red-500">Honda Civic</a></li>
              <li><a href="#" className="hover:text-red-500">Honda CR-V</a></li>
              <li><a href="#" className="hover:text-red-500">Honda Civic Type R</a></li>
            </ul>
          </div>

          {/* Services */}
          <div>
            <h3 className="mb-4 font-bold text-white">Dịch vụ</h3>
            <ul className="space-y-2 text-sm">
              <li><a href="#" className="hover:text-red-500">Đăng ký lái thử</a></li>
              <li><a href="#" className="hover:text-red-500">Bảo hiểm Honda</a></li>
              <li><a href="#" className="hover:text-red-500">Dịch vụ sau bán hàng</a></li>
              <li><a href="#" className="hover:text-red-500">Phụ tùng & Phụ kiện</a></li>
              <li><a href="#" className="hover:text-red-500">Xe qua sử dụng</a></li>
              <li><a href="#" className="hover:text-red-500">Tư vấn tài chính</a></li>
            </ul>
          </div>

          {/* Contact */}
          <div>
            <h3 className="mb-4 font-bold text-white">Liên hệ</h3>
            <ul className="space-y-3 text-sm">
              <li className="flex gap-2">
                <MapPin className="h-5 w-5 shrink-0 text-red-500" />
                <span>Số 1 Đường ABC, Quận 1, TP.HCM</span>
              </li>
              <li className="flex gap-2">
                <Phone className="h-5 w-5 shrink-0 text-red-500" />
                <span>1800 1524 (Miễn phí)</span>
              </li>
              <li className="flex gap-2">
                <Mail className="h-5 w-5 shrink-0 text-red-500" />
                <span>info@honda.com.vn</span>
              </li>
            </ul>
          </div>
        </div>
      </div>

      {/* Bottom Bar */}
      <div className="border-t border-gray-800">
        <div className="mx-auto max-w-7xl px-4 py-6">
          <div className="flex flex-col items-center justify-between gap-4 md:flex-row">
            <p className="text-sm">
              © 2026 Honda Vietnam. All rights reserved.
            </p>
            <div className="flex gap-6 text-sm">
              <a href="#" className="hover:text-red-500">Chính sách bảo mật</a>
              <a href="#" className="hover:text-red-500">Điều khoản sử dụng</a>
              <a href="#" className="hover:text-red-500">Sitemap</a>
            </div>
          </div>
        </div>
      </div>

      {/* Floating Support Button */}
      <div className="fixed bottom-6 right-6 z-50">
        <button className="flex h-14 w-14 items-center justify-center rounded-full bg-red-600 text-white shadow-lg transition hover:bg-red-700 hover:shadow-xl">
          <Phone className="h-6 w-6" />
        </button>
      </div>
    </footer>
  );
}
