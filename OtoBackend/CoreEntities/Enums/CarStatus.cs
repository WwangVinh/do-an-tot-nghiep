namespace CoreEntities.Models
{
    public enum CarStatus
    {
        Draft = 0,              // Bản nháp (Nhân viên đang nhập thông số, chưa xong)
        PendingApproval = 1,    // Chờ quản lý duyệt (Nhân viên ấn nộp)
        Available = 2,          // Đang bán (Quản lý đã duyệt -> Hiện lên Web)
        Out_of_stock = 3,       // Hết hàng
        COMING_SOON = 4,        // Sắp về
        Rejected = 5            // Từ chối (Quản lý chê nhập sai giá, trả về sửa)
    }
}