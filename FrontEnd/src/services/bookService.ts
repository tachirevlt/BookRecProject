import api from './api';
import type { Book } from '../types/Book';

export const bookService = {

  getAll: async (page: number = 2, limit: number = 4) => {
    const response = await api.get(`/Books?pageNumber=${page}&pageSize=${limit}`);
    
    return response.data;
  },

  getById: async (id: string): Promise<Book> => {

    const response = await api.get(`/Books/${id}`);
    const data = response.data;

    console.log("🔍 Dữ liệu chi tiết sách gốc:", data); 
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