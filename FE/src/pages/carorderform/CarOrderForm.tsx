import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { env } from '../../lib/env'; // Tuỳ đường dẫn của ní

export function CarOrderForm({ carId, pricingVersions }) {
  // --- STATE QUẢN LÝ FORM ---
  const [step, setStep] = useState(1); // 1: Thông tin, 2: Phụ kiện
  const [formData, setFormData] = useState({
    fullName: '', phone: '', email: '', customerNote: '', promotionCode: ''
  });
  
  // --- STATE QUẢN LÝ TIỀN BẠC ---
  const [selectedVersion, setSelectedVersion] = useState(pricingVersions?.[0] || null);
  const [discountPercent, setDiscountPercent] = useState(0);
  const [discountAmount, setDiscountAmount] = useState(0);
  const [promoMessage, setPromoMessage] = useState({ type: '', text: '' });

  // --- STATE QUẢN LÝ PHỤ KIỆN ---
  const [accessoriesList, setAccessoriesList] = useState([]); // Call API lấy list
  const [selectedAccessories, setSelectedAccessories] = useState([]);
  
  const [loading, setLoading] = useState(false);

  // Lấy danh sách phụ kiện (Có thể gọi useEffect 1 lần lúc component mount)
  useEffect(() => {
    // axios.get(`${env.VITE_API_BASE_URL}/api/public/accessories`).then(res => setAccessoriesList(res.data));
    // Dữ liệu mock tạm thời:
    setAccessoriesList([
        { accessoryId: 1, name: 'Thảm lót sàn 6D', price: 1500000 },
        { accessoryId: 2, name: 'Phim cách nhiệt 3M', price: 8000000 },
        { accessoryId: 3, name: 'Camera hành trình', price: 3500000 }
    ]);
  }, []);

  // Hàm tính toán lại tiền khi đổi Phiên bản xe hoặc Áp mã
  useEffect(() => {
    if (selectedVersion) {
        const discount = (selectedVersion.priceVnd * discountPercent) / 100;
        setDiscountAmount(discount);
    }
  }, [selectedVersion, discountPercent]);

  // --- HÀM XỬ LÝ ---
  const handleApplyPromo = async () => {
    if (!formData.promotionCode) return;
    try {
        const res = await axios.get(`${env.VITE_API_BASE_URL}/api/public/orders/check-promotion?code=${formData.promotionCode}&carId=${carId}`);
        setDiscountPercent(res.data.discountPercentage);
        setPromoMessage({ type: 'success', text: `Áp dụng thành công! Giảm ${res.data.discountPercentage}%` });
    } catch (error) {
        setDiscountPercent(0);
        setPromoMessage({ type: 'error', text: error.response?.data?.message || 'Mã không hợp lệ' });
    }
  };

  const toggleAccessory = (acc) => {
    if (selectedAccessories.find(a => a.accessoryId === acc.accessoryId)) {
        setSelectedAccessories(selectedAccessories.filter(a => a.accessoryId !== acc.accessoryId));
    } else {
        setSelectedAccessories([...selectedAccessories, acc]);
    }
  };

  const handleCheckout = async () => {
    setLoading(true);
    try {
        const payload = {
            ...formData,
            carId: carId,
            accessoryIds: selectedAccessories.map(a => a.accessoryId)
        };
        // Gọi API Checkout mà ní đã test Swagger
        const res = await axios.post(`${env.VITE_API_BASE_URL}/api/public/orders/checkout`, payload);
        
        // Nhảy sang trang PayOS để đặt cọc
        if(res.data.checkoutUrl) {
            window.location.href = res.data.checkoutUrl;
        }
    } catch (error) {
        alert("Lỗi khi tạo đơn: " + (error.response?.data?.message || error.message));
    } finally {
        setLoading(false);
    }
  };

  // --- TÍNH TỔNG TIỀN CUỐI CÙNG ---
  const accessoriesTotal = selectedAccessories.reduce((sum, item) => sum + item.price, 0);
  const finalPrice = (selectedVersion?.priceVnd || 0) - discountAmount + accessoriesTotal;

  return (
    <div className="bg-white p-6 rounded-xl border border-gray-100 shadow-sm w-full">
      
      {/* ================= BƯỚC 1: THÔNG TIN CÁ NHÂN ================= */}
      {step === 1 && (
        <div className="animate-fade-in">
            <h3 className="text-lg font-bold text-[#0A2540] mb-4">THÔNG TIN ĐẶT MUA</h3>
            
            <div className="mb-4">
                <label className="block text-xs font-semibold text-gray-500 mb-1">CHỌN PHIÊN BẢN *</label>
                <select 
                    className="w-full border border-gray-300 rounded p-3 text-sm focus:outline-blue-500"
                    onChange={(e) => setSelectedVersion(pricingVersions.find(v => v.pricingVersionId == e.target.value))}
                >
                    {pricingVersions?.map(v => (
                        <option key={v.pricingVersionId} value={v.pricingVersionId}>
                            {v.name} — {new Intl.NumberFormat('vi-VN').format(v.priceVnd)} đ
                        </option>
                    ))}
                </select>
            </div>

            <div className="grid grid-cols-2 gap-4 mb-4">
                <div>
                    <label className="block text-xs font-semibold text-gray-500 mb-1">HỌ VÀ TÊN *</label>
                    <input type="text" className="w-full border rounded p-3 text-sm" placeholder="Nguyễn Văn A" 
                        value={formData.fullName} onChange={e => setFormData({...formData, fullName: e.target.value})} />
                </div>
                <div>
                    <label className="block text-xs font-semibold text-gray-500 mb-1">SỐ ĐIỆN THOẠI *</label>
                    <input type="text" className="w-full border rounded p-3 text-sm" placeholder="0987 654 321"
                        value={formData.phone} onChange={e => setFormData({...formData, phone: e.target.value})} />
                </div>
            </div>

            {/* PHẦN ÁP MÃ GIẢM GIÁ */}
            <div className="mb-4">
                <label className="block text-xs font-semibold text-gray-500 mb-1">MÃ KHUYẾN MÃI</label>
                <div className="flex gap-2">
                    <input type="text" className="flex-1 border rounded p-3 text-sm uppercase" placeholder="VD: SALE2026"
                        value={formData.promotionCode} onChange={e => setFormData({...formData, promotionCode: e.target.value})} />
                    <button className="bg-gray-800 text-white px-4 rounded text-sm font-semibold" onClick={handleApplyPromo}>
                        ÁP DỤNG
                    </button>
                </div>
                {promoMessage.text && (
                    <p className={`text-xs mt-1 ${promoMessage.type === 'error' ? 'text-red-500' : 'text-green-600 font-bold'}`}>
                        {promoMessage.text}
                    </p>
                )}
            </div>

            {/* HIỂN THỊ TIỀN SAU KHI ÁP MÃ */}
            <div className="bg-gray-50 p-4 rounded mb-4">
                <div className="flex justify-between text-sm mb-1 text-gray-600">
                    <span>Giá xe:</span>
                    <span>{new Intl.NumberFormat('vi-VN').format(selectedVersion?.priceVnd || 0)} đ</span>
                </div>
                {discountAmount > 0 && (
                    <div className="flex justify-between text-sm mb-1 text-green-600 font-semibold">
                        <span>Giảm giá ({discountPercent}%):</span>
                        <span>- {new Intl.NumberFormat('vi-VN').format(discountAmount)} đ</span>
                    </div>
                )}
                <div className="flex justify-between text-base font-bold text-[#0A2540] mt-2 pt-2 border-t border-gray-200">
                    <span>Tạm tính:</span>
                    <span>{new Intl.NumberFormat('vi-VN').format((selectedVersion?.priceVnd || 0) - discountAmount)} đ</span>
                </div>
            </div>

            <button 
                className="w-full bg-[#0A2540] text-white font-bold py-3 rounded hover:bg-[#0d345c] transition-all"
                onClick={() => setStep(2)}
            >
                TIẾP TỤC (CHỌN PHỤ KIỆN)
            </button>
        </div>
      )}

      {/* ================= BƯỚC 2: CHỌN PHỤ KIỆN ================= */}
      {step === 2 && (
        <div className="animate-fade-in">
            <div className="flex items-center gap-2 mb-4 text-[#0A2540]">
                <button onClick={() => setStep(1)} className="text-gray-400 hover:text-gray-800">
                   ← Quay lại
                </button>
                <h3 className="text-lg font-bold ml-auto">PHỤ KIỆN MUA KÈM</h3>
            </div>

            <div className="max-h-60 overflow-y-auto mb-4 border border-gray-100 rounded p-2">
                {accessoriesList.map(acc => {
                    const isSelected = selectedAccessories.some(a => a.accessoryId === acc.accessoryId);
                    return (
                        <div key={acc.accessoryId} 
                            className={`flex items-center justify-between p-3 mb-2 rounded border cursor-pointer transition-all ${isSelected ? 'border-blue-500 bg-blue-50' : 'border-gray-200 hover:bg-gray-50'}`}
                            onClick={() => toggleAccessory(acc)}
                        >
                            <div className="flex items-center gap-3">
                                <input type="checkbox" checked={isSelected} readOnly className="w-4 h-4" />
                                <span className="text-sm font-medium">{acc.name}</span>
                            </div>
                            <span className="text-sm font-semibold text-gray-700">
                                +{new Intl.NumberFormat('vi-VN').format(acc.price)} đ
                            </span>
                        </div>
                    )
                })}
            </div>

            {/* TỔNG KẾT TIỀN */}
            <div className="bg-[#0A2540] text-white p-4 rounded mb-4">
                <div className="flex justify-between text-sm mb-1 opacity-80">
                    <span>Xe (Sau giảm giá):</span>
                    <span>{new Intl.NumberFormat('vi-VN').format((selectedVersion?.priceVnd || 0) - discountAmount)} đ</span>
                </div>
                <div className="flex justify-between text-sm mb-1 opacity-80">
                    <span>Phụ kiện ({selectedAccessories.length}):</span>
                    <span>+ {new Intl.NumberFormat('vi-VN').format(accessoriesTotal)} đ</span>
                </div>
                <div className="flex justify-between text-lg font-bold mt-3 pt-3 border-t border-white/20">
                    <span>TỔNG CỘNG:</span>
                    <span className="text-amber-400">{new Intl.NumberFormat('vi-VN').format(finalPrice)} đ</span>
                </div>
            </div>

            <button 
                className="w-full bg-blue-600 text-white font-bold py-3 rounded hover:bg-blue-700 transition-all flex justify-center items-center gap-2"
                onClick={handleCheckout}
                disabled={loading}
            >
                {loading ? 'ĐANG TẠO ĐƠN...' : 'XÁC NHẬN ĐẶT CỌC QUA PAYOS'}
            </button>
        </div>
      )}

    </div>
  );
}