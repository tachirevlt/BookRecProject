import axios from 'axios';

// Đảm bảo URL này đúng với Controller của bạn (UsersController)
const API_URL = 'http://localhost:5227/api/Users'; 

// Hàm giải mã Token (Giữ nguyên)
function parseJwt (token: string) {
    try {
        var base64Url = token.split('.')[1];
        var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        var jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function(c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));

        return JSON.parse(jsonPayload);
    } catch (e) {
        return null;
    }
}

export const authService = {
  // 1. Đăng nhập
  login: async (username: string, password: string) => { // Lưu ý: Backend bạn dùng username hay email thì truyền cái đó
    const response = await axios.post(`${API_URL}/login`, { username, password });
    if (response.data) {
        localStorage.setItem('user', JSON.stringify(response.data));
    }
    return response.data;
  },

  // 2. Đăng ký
  register: async (username: string, email: string, password: string) => {
    return await axios.post(`${API_URL}/register`, { username, email, password });
  },

  // 3. Đăng xuất
  logout: () => {
    localStorage.removeItem('user'); 
  },

  // 4. Lấy User hiện tại (QUAN TRỌNG NHẤT)
  getCurrentUser: () => {
    const userStr = localStorage.getItem('user');
    if (!userStr) return null;

    const user = JSON.parse(userStr);
    const token = user.token || user.accessToken || user.Token; 
    
    if (token) {
        const decoded = parseJwt(token);
        
        // --- LOGIC LẤY TÊN (USERNAME) ---
        const realName = 
            decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || // Chuẩn .NET
            decoded["unique_name"] || 
            decoded["name"] || 
            user.username;

        // --- LOGIC LẤY ID (USERID) - BẮT BUỘC PHẢI CÓ ---
        const realId = 
            decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || // Chuẩn .NET cho ID
            decoded["nameid"] ||
            decoded["sub"] || 
            decoded["id"] ||
            user.userId;

        return {
            ...user,
            username: realName, 
            userId: realId // Trả về userId chuẩn để dùng gọi API Profile
        };
    }

    return user;
  }
};