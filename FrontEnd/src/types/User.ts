import type { Book } from './Book';

export interface User {
  UserId: string; 
  Username: string;
  Email: string;
  Role: string;
  
  FavoriteBooks?: Book[]; 
}