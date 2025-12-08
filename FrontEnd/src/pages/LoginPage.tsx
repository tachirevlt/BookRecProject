import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { authService } from '../services/authService';

export const LoginPage = () => {
  const [username, setUserName] = useState('');
  const [password, setPassword] = useState('');
  const navigate = useNavigate();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await authService.login(username, password);
      alert("Đăng nhập thành công! 🎉");
      navigate('/'); // Chuyển về trang chủ
      window.location.reload(); // Load lại trang để cập nhật Header
    } catch (error: any) {
      alert("Đăng nhập thất bại: " + (error.response?.data || error.message));
    }
  };

  return (
    <div style={styles.container}>
      <div style={styles.card}>
        <h2 style={{textAlign: 'center', color: '#333'}}>Đăng Nhập</h2>
        <form onSubmit={handleLogin} style={{display: 'flex', flexDirection: 'column', gap: '15px'}}>
          <input 
            type="username" placeholder="Username" required style={styles.input}
            value={username} onChange={e => setUserName(e.target.value)}
          />
          <input 
            type="password" placeholder="Mật khẩu" required style={styles.input}
            value={password} onChange={e => setPassword(e.target.value)}
          />
          <button type="submit" style={styles.button}>Đăng nhập</button>
        </form>
        <p style={{textAlign: 'center', marginTop: '15px'}}>
          Chưa có tài khoản? <Link to="/register">Đăng ký ngay</Link>
        </p>
      </div>
    </div>
  );
};

// CSS viết gọn
const styles = {
  container: { display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh', background: '#f0f2f5' },
  card: { background: 'white', padding: '40px', borderRadius: '10px', boxShadow: '0 4px 10px rgba(0,0,0,0.1)', width: '350px' },
  input: { padding: '10px', borderRadius: '5px', border: '1px solid #ddd', fontSize: '16px' },
  button: { padding: '10px', background: '#1877f2', color: 'white', border: 'none', borderRadius: '5px', cursor: 'pointer', fontSize: '16px', fontWeight: 'bold' as 'bold' }
};