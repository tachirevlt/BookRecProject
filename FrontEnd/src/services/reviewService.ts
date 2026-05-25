import axiosClient from './axiosClient';
import { ENDPOINTS } from './endpoints';

// ─── TYPES ───────────────────────────────────────────────────────────────────

export interface ReviewData {
  id: string;
  user_id: string;
  book_id: string;
  review: string;
  time: string;
}

export interface AddReviewPayload {
  book_id: string;
  Review: string; // Viết hoa chữ R theo đúng payload Backend yêu cầu trong endpoints.ts
}

// ─── SERVICE ─────────────────────────────────────────────────────────────────

export const reviewService = {
  /**
   * Lấy danh sách đánh giá của một cuốn sách
   * GET /api/reviews/book/:book_id
   */
  getReviewsByBook: async (bookId: string): Promise<ReviewData[]> => {
    const response = await axiosClient.get<ReviewData[]>(
      ENDPOINTS.REVIEWS.GET_BY_BOOK(bookId)
    );
    return response.data;
  },

  /**
   * Thêm đánh giá mới
   * POST /api/reviews
   */
  addReview: async (payload: AddReviewPayload): Promise<ReviewData> => {
    const response = await axiosClient.post<ReviewData>(
      ENDPOINTS.REVIEWS.ADD,
      payload
    );
    return response.data;
  },

  /**
   * Xóa đánh giá (Chỉ owner hoặc admin)
   * DELETE /api/reviews/:review_id
   */
  deleteReview: async (reviewId: string): Promise<void> => {
    await axiosClient.delete(ENDPOINTS.REVIEWS.DELETE(reviewId));
  }
};