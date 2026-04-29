import type { NewsPost } from './types'

import banner1 from '../../assets/images/banner1.jpg'
import banner2 from '../../assets/images/banner2.jpg'
import banner3 from '../../assets/images/banner3.jpg'
import banner4 from '../../assets/images/banner4.jpg'
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

const imgs = [
  banner1,
  banner2,
  banner3,
  banner4,
  vf3Img,
  vf5Img,
  vf6Img,
  vf7Img,
  vf8Img,
  vf9Img,
  minioGreenImg,
  herioGreenImg,
  nerioGreenImg,
  limoGreenImg,
  ecVanImg,
] as const

function imgAt(i: number) {
  return imgs[i % imgs.length]!
}

/** Danh sách bài viết chính (cột trái) */
export const mockNewsPosts: NewsPost[] = [
  {
    id: 'post-1',
    title: 'THỂ LỆ CUỘC THI “LÁI XE XANH – RINH RỒNG VÀNG”',
    excerpt:
      'Diễn ra từ 24/01 đến 19/3/2024, Cuộc thi “Lái xe xanh – Rinh rồng vàng” dành cho khách hàng và chủ xe VinFast với nhiều phần quà giá trị [...]',
    imageSrc: imgAt(0),
    href: '#',
  },
  {
    id: 'post-2',
    title: 'VinFast VF 6 thu hút đông đảo khách tham quan và đặt cọc tại TP.Hồ Chí Minh',
    excerpt:
      'Chiếc VF 6 màu hồng nổi bật tại Vinhomes Landmark 81, nhận được sự quan tâm lớn từ khách hàng trong ngày mở cửa trưng bày [...]',
    imageSrc: imgAt(1),
    href: '#',
  },
  {
    id: 'post-3',
    title: 'VinFast chính thức nhận đặt cọc xe VF 6 – Ưu đãi 20 triệu đồng, miễn phí 1 năm sạc pin',
    excerpt:
      'Chương trình ưu đãi áp dụng trong thời gian giới hạn, kèm gói hỗ trợ sạc và dịch vụ hậu mãi đi kèm [...]',
    imageSrc: imgAt(2),
    href: '#',
  },
  {
    id: 'post-4',
    title: 'Ra mắt dòng VF mới: trải nghiệm lái thử tại hệ thống đại lý toàn quốc',
    excerpt:
      'Khách hàng đăng ký lái thử trực tuyến hoặc tại showroom để cảm nhận công nghệ pin và hệ thống ADAS trên xe điện VinFast [...]',
    imageSrc: imgAt(3),
    href: '#',
  },
  {
    id: 'post-5',
    title: 'VinFast VF 3 – mẫu xe đô thị gọn nhẹ, phù hợp di chuyển hằng ngày',
    excerpt:
      'VF 3 hướng tới khách hàng cá nhân và gia đình trẻ, tối ưu bán kính quay đầu và chi phí vận hành [...]',
    imageSrc: imgAt(4),
    href: '#',
  },
  {
    id: 'post-6',
    title: 'Chính sách bảo hành pin mới: an tâm cho hành trình dài',
    excerpt:
      'Cập nhật điều kiện bảo hành và hỗ trợ thay thế linh kiện, giúp chủ xe yên tâm sử dụng trong nhiều năm [...]',
    imageSrc: imgAt(5),
    href: '#',
  },
  {
    id: 'post-7',
    title: 'Khai trương trạm sạc nhanh tại khu vực trung tâm – mở rộng mạng lưới',
    excerpt:
      'Trạm sạc DC hỗ trợ nhiều chuẩn cổng, rút ngắn thời gian nạp điện cho khách hàng đi làm và du lịch [...]',
    imageSrc: imgAt(6),
    href: '#',
  },
  {
    id: 'post-8',
    title: 'Ưu đãi đặc biệt quý IV: hỗ trợ lệ phí trước bạ và gói phụ kiện',
    excerpt:
      'Áp dụng theo từng dòng xe và thời điểm, chi tiết liên hệ tư vấn viên để nhận báo giá tốt nhất [...]',
    imageSrc: imgAt(7),
    href: '#',
  },
  {
    id: 'post-9',
    title: 'VinFast VF 9 – không gian rộng rãi, lựa chọn cho gia đình đa thế hệ',
    excerpt:
      'Hàng ghế linh hoạt, cốp lớn và hệ thống giải trí giúp hành trình dài thêm thoải mái [...]',
    imageSrc: imgAt(8),
    href: '#',
  },
  {
    id: 'post-10',
    title: 'Dòng xe dịch vụ Green: giải pháp vận tải bền vững cho doanh nghiệp',
    excerpt:
      'Các mẫu Minio Green, Herio Green phù hợp taxi, dịch vụ giao hàng và thuê xe có tài [...]',
    imageSrc: imgAt(9),
    href: '#',
  },
  {
    id: 'post-11',
    title: 'Hội thảo “Xe điện và tương lai giao thông xanh” tại TP.HCM',
    excerpt:
      'Sự kiện quy tụ chuyên gia và đối tác, chia sẻ định hướng phát triển hạ tầng sạc và chính sách hỗ trợ [...]',
    imageSrc: imgAt(10),
    href: '#',
  },
  {
    id: 'post-12',
    title: 'Cập nhật phần mềm OTA: cải thiện hiển thị và tính năng an toàn',
    excerpt:
      'Khách hàng được thông báo cập nhật qua ứng dụng, nên kết nối Wi-Fi ổn định khi cài đặt [...]',
    imageSrc: imgAt(11),
    href: '#',
  },
  {
    id: 'post-13',
    title: 'Chương trình đổi cũ lấy mới – hỗ trợ định giá xe cũ minh bạch',
    excerpt:
      'Quy trình thẩm định nhanh, hỗ trợ sang tên và khấu trừ trực tiếp vào hợp đồng mua xe mới [...]',
    imageSrc: imgAt(12),
    href: '#',
  },
  {
    id: 'post-14',
    title: 'EC Van – lựa chọn tối ưu cho logistics và vận chuyển “last mile”',
    excerpt:
      'Thùng chở rộng, vận hành êm, chi phí năng lượng thấp so với xe xăng cùng phân khúc [...]',
    imageSrc: imgAt(13),
    href: '#',
  },
  {
    id: 'post-15',
    title: 'Lịch sự kiện lái thử cuối tuần tại showroom VinFast Minh Đạo',
    excerpt:
      'Đăng ký theo khung giờ để được hướng dẫn lái và trải nghiệm tính năng trên đường thử an toàn [...]',
    imageSrc: imgAt(14),
    href: '#',
  },
]

/** Bài viết mới (sidebar): rút gọn tiêu đề + ngày */
export const mockNewsLatest: NewsPost[] = [
  { id: 'latest-1', title: mockNewsPosts[0]!.title, dateText: '12 Tháng 1, 2026', imageSrc: imgAt(0), href: '#' },
  { id: 'latest-2', title: mockNewsPosts[1]!.title, dateText: '10 Tháng 1, 2026', imageSrc: imgAt(1), href: '#' },
  { id: 'latest-3', title: mockNewsPosts[2]!.title, dateText: '8 Tháng 1, 2026', imageSrc: imgAt(2), href: '#' },
  { id: 'latest-4', title: mockNewsPosts[3]!.title, dateText: '5 Tháng 1, 2026', imageSrc: imgAt(3), href: '#' },
  { id: 'latest-5', title: mockNewsPosts[4]!.title, dateText: '2 Tháng 1, 2026', imageSrc: imgAt(4), href: '#' },
  { id: 'latest-6', title: mockNewsPosts[5]!.title, dateText: '28 Tháng 12, 2025', imageSrc: imgAt(5), href: '#' },
  { id: 'latest-7', title: mockNewsPosts[6]!.title, dateText: '22 Tháng 12, 2025', imageSrc: imgAt(6), href: '#' },
  { id: 'latest-8', title: mockNewsPosts[7]!.title, dateText: '18 Tháng 12, 2025', imageSrc: imgAt(7), href: '#' },
  { id: 'latest-9', title: mockNewsPosts[8]!.title, dateText: '15 Tháng 12, 2025', imageSrc: imgAt(8), href: '#' },
  { id: 'latest-10', title: mockNewsPosts[9]!.title, dateText: '10 Tháng 12, 2025', imageSrc: imgAt(9), href: '#' },
  { id: 'latest-11', title: mockNewsPosts[10]!.title, dateText: '5 Tháng 12, 2025', imageSrc: imgAt(10), href: '#' },
  { id: 'latest-12', title: mockNewsPosts[11]!.title, dateText: '1 Tháng 12, 2025', imageSrc: imgAt(11), href: '#' },
]
