import ecVanImg from '../../assets/images/cars/vinfast-ec-van-9po1yr5.webp'
import herioGreenImg from '../../assets/images/cars/vinfast-herio-green-bzfpycc.webp'
import limoGreenImg from '../../assets/images/cars/vinfast-limo-green-r7r4yb9.webp'
import minioGreenImg from '../../assets/images/cars/vinfast-minio-green-oeabyca.webp'
import nerioGreenImg from '../../assets/images/cars/vinfast-nerio-green-b2s1ycb.webp'
import vf3Img from '../../assets/images/cars/vinfast-vf3-bi0bv4h.webp'
import vf5Img from '../../assets/images/cars/vinfast-vf5-2b5ssay.webp'
import vf6Img from '../../assets/images/cars/vinfast-vf6-b1tsur6.webp'
import vf7Img from '../../assets/images/cars/vinfast-vf7-rtqsur7.webp'
import vf8Img from '../../assets/images/cars/vinfast-vf8-98yirhq.webp'
import vf9Img from '../../assets/images/cars/vinfast-vf9-24xfrhp.webp'

export type CarProductSpecRow = { category?: string; label: string; value: string }

export type CarProductGallerySlide = {
  src: string
  alt: string
  title?: string | null
  description?: string | null
}

export type CarProductPricingRow = { name: string; priceText: string }

/** Ảnh theo từng màu (category Color trong gallery API). */
export type CarProductColorGalleryItem = { id: string; label: string; imageSrc: string }

/** Một mục: carousel trái + mô tả tổng hợp phải (Overview, Ngoại thất, …). */
export type CarProductSectionSplit = {
  intro: string
  slides: CarProductGallerySlide[]
  bodyText: string
}

export type CarProductMeta = {
  name: string
  imageSrc: string
  priceText: string
  /** Nội dung demo để render 7 mục trong trang chi tiết. */
  content: {
    overviewIntro: string
    overviewHighlights: string[]
    exteriorIntro: string
    exteriorBullets: string[]
    interiorIntro: string
    interiorBullets: string[]
    safetyIntro: string
    safetyBullets: string[]
    performanceIntro: string
    performanceBullets: string[]
    specsTitle?: string
    specsRows: CarProductSpecRow[]
    gallery?: CarProductGallerySlide[]
    /**Toàn bộ ảnh gallery (mọi category) cho mục “Hình ảnh”. */
    galleryAll?: CarProductGallerySlide[]
    /** Bảng giá theo phiên bản; nếu không có thì dùng một dòng `name` + `priceText`. */
    pricingRows?: CarProductPricingRow[]
    /** Màu xe lấy từ gallery Color (ảnh thumbnail + đổi ảnh hero). */
    colorGallery?: CarProductColorGalleryItem[]
    overviewSplit?: CarProductSectionSplit
    exteriorSplit?: CarProductSectionSplit
    interiorSplit?: CarProductSectionSplit
    safetySplit?: CarProductSectionSplit
    performanceSplit?: CarProductSectionSplit
  }
}

const demoBase = (name: string): CarProductMeta['content'] => ({
  overviewIntro: `${name} là mẫu xe điện VinFast với thiết kế hiện đại, phù hợp di chuyển đô thị và gia đình. Nội dung dưới đây là demo để bạn dễ hoàn thiện theo từng phiên bản.`,
  overviewHighlights: [
    'Thiết kế tối ưu khí động học, nhận diện thương hiệu rõ nét',
    'Hệ sinh thái sạc & dịch vụ theo chính sách hãng',
    'Kết nối và tiện nghi theo cấu hình từng phiên bản',
  ],
  exteriorIntro:
    'Ngoại thất hướng tới sự hiện đại và thực dụng: tối ưu luồng gió, tăng hiệu quả vận hành và giữ dáng xe cân đối.',
  exteriorBullets: [
    'Cụm đèn LED đặc trưng, tăng khả năng quan sát',
    'Mâm hợp kim đa chấu (tùy phiên bản)',
    'Đường gân thân xe khỏe khoắn, nhấn mạnh tính thể thao',
  ],
  interiorIntro:
    'Nội thất tối giản, tập trung trải nghiệm người dùng; vật liệu và trang bị tùy theo phiên bản và gói tuỳ chọn.',
  interiorBullets: [
    'Màn hình trung tâm và hệ thống giải trí theo cấu hình',
    'Ghế ngồi tối ưu tư thế, không gian linh hoạt',
    'Kết nối điện thoại thông minh (tùy phiên bản)',
  ],
  safetyIntro:
    'Trang bị an toàn chủ động/bị động tuân theo tiêu chuẩn của hãng; các tính năng nâng cao có thể khác nhau theo phiên bản.',
  safetyBullets: [
    'ABS/EBD/ESC và hỗ trợ phanh (tùy phiên bản)',
    'Cảnh báo điểm mù, hỗ trợ giữ làn (tùy phiên bản)',
    'Cảnh báo va chạm phía trước & hỗ trợ phanh khẩn cấp (tùy phiên bản)',
    'Kiểm soát hành trình thích ứng, giới hạn tốc độ thông minh (tùy phiên bản)',
    'Túi khí và kết cấu khung xe tối ưu hấp thụ lực',
    'Camera/cảm biến hỗ trợ đỗ xe, cảnh báo phương tiện cắt ngang (tùy phiên bản)',
  ],
  performanceIntro:
    'Vận hành êm, phản hồi chân ga nhanh và tối ưu cho di chuyển hằng ngày; khả năng sạc AC/DC tùy theo mẫu xe.',
  performanceBullets: [
    'Động cơ điện phản hồi tức thì, tăng tốc mượt',
    'Khung gầm/tối ưu phân bổ lực kéo giúp ổn định thân xe',
    'Chế độ lái tùy chỉnh (Eco/Normal/Sport...) theo phiên bản',
    'Tái tạo năng lượng khi phanh giúp tối ưu hiệu suất (tùy mẫu xe)',
    'Hỗ trợ sạc theo chuẩn trạm sạc của hãng (tùy mẫu xe)',
    'Cách âm và độ êm được tinh chỉnh cho hành trình dài (tùy cấu hình)',
  ],
  specsTitle: 'Thông số kỹ thuật',
  specsRows: [
    { label: 'Giá từ (tham khảo)', value: 'Theo bảng giá niêm yết & ưu đãi tháng' },
    { label: 'Tư vấn phiên bản', value: 'Liên hệ showroom để nhận catalogue & cấu hình chi tiết' },
    { label: 'Bảo hành', value: 'Theo chính sách VinFast tại thời điểm mua' },
  ],
  gallery: [],
})

export const CAR_PRODUCT_CATALOG: Record<string, CarProductMeta> = {
  vf3: {
    name: 'VinFast VF3',
    imageSrc: vf3Img,
    priceText: '281 triệu',
    content: { ...demoBase('VinFast VF3') },
  },
  vf5: {
    name: 'VinFast VF5',
    imageSrc: vf5Img,
    priceText: '497 triệu',
    content: { ...demoBase('VinFast VF5') },
  },
  vf6: {
    name: 'VinFast VF6',
    imageSrc: vf6Img,
    priceText: '647 triệu',
    content: { ...demoBase('VinFast VF6') },
  },
  vf7: {
    name: 'VinFast VF 7',
    imageSrc: vf7Img,
    priceText: 'Tham khảo bảng giá',
    content: {
      ...demoBase('VinFast VF 7'),
      overviewIntro:
        'VF 7 là mẫu SUV điện cỡ C với thiết kế hiện đại, cân bằng giữa hiệu năng, phạm vi hoạt động và trải nghiệm công nghệ. Nội dung hiện là demo để bạn thay bằng dữ liệu chuẩn.',
      safetyBullets: [
        'Gói ADAS nâng cao: hỗ trợ giữ làn, cảnh báo lệch làn, cảnh báo va chạm (tùy phiên bản)',
        'Kiểm soát hành trình thích ứng, hỗ trợ đi trong điều kiện ùn tắc (tùy phiên bản)',
        'Camera 360 và cảm biến quanh xe hỗ trợ quan sát (tùy phiên bản)',
        'ABS/EBD/ESC và hỗ trợ phanh khẩn cấp',
        'Hệ thống túi khí và cấu trúc khung xe tăng độ cứng vững',
      ],
      performanceBullets: [
        'Phản hồi chân ga nhanh, tăng tốc mượt trong dải tốc độ đô thị',
        'Nhiều chế độ lái tùy chọn theo nhu cầu (tùy phiên bản)',
        'Tái tạo năng lượng khi phanh — tối ưu hiệu suất sử dụng',
        'Tối ưu phân bổ mô-men giúp bám đường và ổn định thân xe',
        'Hỗ trợ sạc AC/DC theo hệ sinh thái trạm sạc VinFast',
      ],
      specsRows: [
        { label: 'Kích thước tổng thể (D × R × C)', value: '4.545 × 1.890 × 1.643 mm (tham khảo)' },
        { label: 'Chiều dài cơ sở', value: '2.840 mm (tham khảo)' },
        { label: 'Phạm vi hoạt động', value: 'Theo cấu hình/tiêu chuẩn công bố (demo)' },
      ],
    },
  },
  vf8: {
    name: 'VinFast VF8',
    imageSrc: vf8Img,
    priceText: '917 triệu',
    content: { ...demoBase('VinFast VF8') },
  },
  vf9: {
    name: 'VinFast VF9',
    imageSrc: vf9Img,
    priceText: '1.349 triệu',
    content: { ...demoBase('VinFast VF9') },
  },
  'minio-green': {
    name: 'VinFast Minio Green',
    imageSrc: minioGreenImg,
    priceText: '252 triệu',
    content: { ...demoBase('VinFast Minio Green') },
  },
  'herio-green': {
    name: 'VinFast Herio Green',
    imageSrc: herioGreenImg,
    priceText: '469 triệu',
    content: { ...demoBase('VinFast Herio Green') },
  },
  'nerio-green': {
    name: 'VinFast Nerio Green',
    imageSrc: nerioGreenImg,
    priceText: '627 triệu',
    content: { ...demoBase('VinFast Nerio Green') },
  },
  'limo-green': {
    name: 'VinFast Limo Green',
    imageSrc: limoGreenImg,
    priceText: '704 triệu',
    content: { ...demoBase('VinFast Limo Green') },
  },
  'ec-van': {
    name: 'VinFast EC Van',
    imageSrc: ecVanImg,
    priceText: '287 triệu',
    content: { ...demoBase('VinFast EC Van') },
  },
}
