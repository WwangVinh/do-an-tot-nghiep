import { useState, useEffect } from "react";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { motion, AnimatePresence } from "motion/react";
import { Button } from "./ui/button";

const slides = [
  {
    id: 1,
    image: "https://images.unsplash.com/photo-1644749700856-a82a92828a1b?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxtb2Rlcm4lMjBjYXIlMjBzaG93cm9vbXxlbnwxfHx8fDE3NzI2MzAyNzF8MA&ixlib=rb-4.1.0&q=80&w=1920",
    title: "Ưu đãi",
    highlight: "LỘC VÀNG RỘN RÀNG DU XUÂN",
    subtitle: "Ưu đãi lên đến",
    offer: "100%",
    offerDetail: "LỆ PHÍ TRƯỚC BẠ",
    additional: "Hỗ trợ lãi suất lên đến",
    additionalValue: "0%"
  },
  {
    id: 2,
    image: "https://images.unsplash.com/photo-1571001437100-9d282569809b?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxsdXh1cnklMjBjYXJzJTIwbGluZXVwfGVufDF8fHx8MTc3MjczNDA1Nnww&ixlib=rb-4.1.0&q=80&w=1920",
    title: "Khuyến mãi đặc biệt",
    highlight: "HONDA CIVIC 2026",
    subtitle: "Giảm giá ngay",
    offer: "50 triệu",
    offerDetail: "KHI MUA XE TRONG THÁNG",
    additional: "Tặng kèm phụ kiện",
    additionalValue: "20 triệu"
  },
  {
    id: 3,
    image: "https://images.unsplash.com/photo-1745524329352-cb462519036e?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxjaGVycnklMjBibG9zc29tJTIwY2FyfGVufDF8fHx8MTc3MjczNDA1Nnww&ixlib=rb-4.1.0&q=80&w=1920",
    title: "Mùa xuân 2026",
    highlight: "TRẢI NGHIỆM LÁI THỬ MIỄN PHÍ",
    subtitle: "Nhận ưu đãi đến",
    offer: "100 triệu",
    offerDetail: "CHO KHÁCH HÀNG ĐẶT TRƯỚC",
    additional: "Giao xe ngay",
    additionalValue: "Tận nhà"
  }
];

export function HeroSlider() {
  const [currentSlide, setCurrentSlide] = useState(0);

  useEffect(() => {
    const timer = setInterval(() => {
      setCurrentSlide((prev) => (prev + 1) % slides.length);
    }, 5000);
    return () => clearInterval(timer);
  }, []);

  const nextSlide = () => {
    setCurrentSlide((prev) => (prev + 1) % slides.length);
  };

  const prevSlide = () => {
    setCurrentSlide((prev) => (prev - 1 + slides.length) % slides.length);
  };

  return (
    <div className="relative h-[500px] overflow-hidden bg-gray-100">
      <AnimatePresence mode="wait">
        <motion.div
          key={currentSlide}
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          transition={{ duration: 0.7 }}
          className="absolute inset-0"
        >
          {/* Background Image */}
          <div
            className="absolute inset-0 bg-cover bg-center"
            style={{ backgroundImage: `url(${slides[currentSlide].image})` }}
          >
            <div className="absolute inset-0 bg-gradient-to-r from-black/60 via-black/40 to-transparent" />
          </div>

          {/* Content */}
          <div className="relative mx-auto flex h-full max-w-7xl items-center px-4">
            <div className="max-w-3xl space-y-6 text-white">
              <motion.div
                initial={{ y: 20, opacity: 0 }}
                animate={{ y: 0, opacity: 1 }}
                transition={{ delay: 0.2 }}
              >
                <p className="text-lg">{slides[currentSlide].title}</p>
                <h1 className="text-5xl font-bold italic text-red-500">
                  {slides[currentSlide].highlight}
                </h1>
              </motion.div>

              <motion.div
                initial={{ y: 20, opacity: 0 }}
                animate={{ y: 0, opacity: 1 }}
                transition={{ delay: 0.4 }}
                className="flex items-center gap-8"
              >
                <div className="rounded-lg bg-blue-600/90 px-8 py-6 backdrop-blur-sm">
                  <p className="text-sm">{slides[currentSlide].subtitle}</p>
                  <p className="text-6xl font-bold text-white">
                    {slides[currentSlide].offer}
                  </p>
                  <p className="mt-1 text-sm font-semibold">
                    {slides[currentSlide].offerDetail}
                  </p>
                </div>

                <div className="text-3xl font-bold">+</div>

                <div className="rounded-lg bg-red-600/90 px-8 py-6 backdrop-blur-sm">
                  <p className="text-sm">{slides[currentSlide].additional}</p>
                  <p className="text-6xl font-bold text-white">
                    {slides[currentSlide].additionalValue}
                  </p>
                </div>
              </motion.div>

              <motion.div
                initial={{ y: 20, opacity: 0 }}
                animate={{ y: 0, opacity: 1 }}
                transition={{ delay: 0.6 }}
                className="flex gap-4"
              >
                <Button className="border-2 border-red-600 bg-red-600 text-white hover:bg-red-700">
                  BẢNG GIÁ SẢN PHẨM →
                </Button>
                <Button variant="outline" className="border-2 border-white bg-transparent text-white hover:bg-white/10">
                  SO SÁNH SẢN PHẨM →
                </Button>
              </motion.div>

              <motion.p
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                transition={{ delay: 0.8 }}
                className="text-xs italic"
              >
                Ưu đãi áp dụng TỪ NGAY 02/03/2026 đến HẾT NGÀY 31/03/2026.<br />
                Các mức ưu đãi khác nhau theo từng Nhà phân phối. Quý Khách vui lòng liên hệ Nhà phân phối gần nhất để biết thêm chi tiết.
              </motion.p>
            </div>
          </div>
        </motion.div>
      </AnimatePresence>

      {/* Navigation Arrows */}
      <button
        onClick={prevSlide}
        className="absolute left-4 top-1/2 -translate-y-1/2 rounded-full bg-white/20 p-2 text-white backdrop-blur-sm transition hover:bg-white/30"
      >
        <ChevronLeft className="h-6 w-6" />
      </button>
      <button
        onClick={nextSlide}
        className="absolute right-4 top-1/2 -translate-y-1/2 rounded-full bg-white/20 p-2 text-white backdrop-blur-sm transition hover:bg-white/30"
      >
        <ChevronRight className="h-6 w-6" />
      </button>

      {/* Dots Indicator */}
      <div className="absolute bottom-6 left-1/2 flex -translate-x-1/2 gap-2">
        {slides.map((_, index) => (
          <button
            key={index}
            onClick={() => setCurrentSlide(index)}
            className={`h-2 rounded-full transition-all ${
              index === currentSlide ? "w-8 bg-white" : "w-2 bg-white/50"
            }`}
          />
        ))}
      </div>

      {/* Slide Counter */}
      <div className="absolute bottom-6 right-6 rounded bg-black/50 px-3 py-1 text-sm text-white backdrop-blur-sm">
        {currentSlide + 1}/{slides.length}
      </div>
    </div>
  );
}
