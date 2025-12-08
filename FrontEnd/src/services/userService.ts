import api from './api';

export interface UserProfile {
  userId: string;
  username: string;
  email: string;
  role: string;
  favoriteBooks: any[]; 
}

export const userService = {
  // 👇 SỬA LẠI ĐÚNG API CỦA BẠN: /api/Users/{id}
  getProfile: async (userId: string): Promise<UserProfile> => {
    // Lưu ý: Có chữ 's' (Users) và truyền ID vào URL
    const response = await api.get(`/Users/${userId}`);
    return response.data;
  },

  // Thêm vào yêu thích
  addFavorite: async (userId: string, bookId: string) => {
    await api.post(`/Users/${userId}/favorites/${bookId}`);
  },

  // Xóa khỏi yêu thích
  removeFavorite: async (userId: string, bookId: string) => {
    await api.delete(`/Users/${userId}/favorites/${bookId}`);
  }
};