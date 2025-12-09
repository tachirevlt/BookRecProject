import api from './api';
import type { Book } from '../types/Book';

export const bookService = {

  // Thêm title và author vào tham số (để trống nếu không tìm)
  getAll: async (page: number = 1, limit: number = 12, genre?: string, searchTerm?: string) => {
    let url = `/Books?pageNumber=${page}&pageSize=${limit}`;
    
    if (genre) url += `&Genre=${encodeURIComponent(genre)}`;
    
    // Bổ sung param Title và Author theo yêu cầu
    if (searchTerm) url += `&SearchTerm=${encodeURIComponent(searchTerm)}`;

    const response = await api.get(url);
    return response.data;
  },

  getById: async (id: string): Promise<Book> => {
    const response = await api.get(`/Books/${id}`);
    const data = response.data;
    if (data.result) return data.result;
    if (data.data) return data.data;
    if (data.value) return data.value;
    if (data.items) return data.items[0];
    return data;
  },

  create: async (book: any): Promise<any> => {
    const response = await api.post('/Books', book); 
    return response.data;
  },

  update: async (id: string, book: Book): Promise<void> => {
    await api.put(`/Books/${id}`, book);
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/Books/${id}`);
  }
};