import api from './api';

export interface UserProfile {
  userId: string;
  username: string;
  email: string;
  role: string;
  favoriteBooks: any[]; 
}

export const userService = {
  getProfile: async (userId: string): Promise<UserProfile> => {
    // Lưu ý: Có chữ 's' (Users) và truyền ID vào URL
    const response = await api.get(`/Users/${userId}`);
    return response.data;
  },

  updateProfile: async (userId: string, data: { username: string; email: string }) => {
    const response = await api.put(`/Users/${userId}`, { 
      id: userId, 
      ...data 
    });
    return response.data;
  },
  // Thêm vào yêu thích
  addFavorite: async (userId: string, bookId: string) => {
    await api.post(`/Users/${userId}/favorites/${bookId}`);
  },

  changePassword: async (userId: string, data: { currentPassword: string; newPassword: string }) => {
    const response = await api.put(`/Users/${userId}/change-password`, {
      id: userId, 
      ...data 
    });
    return response.data;
  },
  // Xóa khỏi yêu thích
  removeFavorite: async (userId: string, bookId: string) => {
    await api.delete(`/Users/${userId}/favorites/${bookId}`);
  }
};