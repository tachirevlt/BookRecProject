import { Routes, Route } from 'react-router-dom';
import { HomePage } from './pages/HomePage';
import { BookFormPage } from './pages/BookFormPage';
import { BookDetailPage } from './pages/BookDetailPage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { UserDetailPage } from './pages/UserDetailPage';
import { UserEditPage } from './pages/UserEditPage';

function App() {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route path="/profile" element={<UserDetailPage />} />
      <Route path="/book/:id" element={<BookDetailPage />} />
      <Route path="/settings" element={<UserEditPage />} />
      <Route path="/create" element={<BookFormPage />} />
      <Route path="/edit/:id" element={<BookFormPage />} />
    </Routes>
  );
}

export default App;